const { build, analyzeMetafile } = require('esbuild')
const pluginVue = require('esbuild-plugin-vue-next')
const { sassPlugin } = require('esbuild-sass-plugin')
const alias = require('esbuild-plugin-path-alias')
const path = require('path')
const fs = require('fs-extra')

//-----------------------------------------
// Example of how to use Dart Sass exe
//-----------------------------------------
/*
const { spawn } = require( 'child_process' );

//"%SCRIPTPATH%\src\dart.exe" "%SCRIPTPATH%\src\sass.snapshot" %arguments%
//./dart.exe sass.snapshot ./src/assets/styles/quidgest.scss ./esdist/quidgest.css -I="../../node_modules"

console.log('Sass started')
let sassTime = new Date();
const sass = spawn( 'dart', [
	'sass.snapshot',
	'./src/assets/styles/quidgest.scss',
	'./esdist/quidgest.css',
	'-I=../../node_modules',
	'--style=compressed'
] );
console.log(`Sass ended: ${new Date() - sassTime}ms`)
*/

/**
 * Esbuild plugin to measure the duration of a build command
 */
let timePlugin = {
	name: 'timer',
	setup(build) {
		let startTime = new Date();
		build.onStart(() => {
			console.log('build started')
			startTime = new Date();
		})
		build.onEnd(result => {
			console.log(`Build ended: ${new Date() - startTime}ms`)
		})
	},
};

//For vendor bundle splitting look at example:
//https://github.com/hyrious/esbuild-split-vendors-example/blob/main/build.js
let pkg = require('./package.json')
let external = Object.keys(pkg.dependencies)

//Adapted from https://www.npmjs.com/package/@esbuild-plugins/esm-externals?activeTab=versions

/**
 * Esbuild plugin to rebase external module calls to their distribution directory
 * @param {Map} externalsMap Map of the aliases of all the external modules, keys are the module, values are the output filename
 * @param {string} outputDir relative path of the distribution directory in relation to the distribution base
 * @returns {Object} The configured plugin
 */
function EsmExternalsPlugin(externalsMap, outputDir) {
	return {
		name: 'esm-externals',
		setup(build) {
			const filterExpr = '^(' + Array.from(externalsMap.keys()).join('|') + ')(\\/.*)?$';
			//console.log(filterExpr);

			build.onResolve({ filter: new RegExp(filterExpr) }, (args) => {
				return {
					path: outputDir + externalsMap.get(args.path),
					external: true,
				}
			})
		},
	};
}

/**
 * Esbuild plugin to collect a map of all the entry points to the respective outputs
 * This plugin works in tandem with the EsmExternalsPlugin by supplying one of its inputs.
 * @param {Map} exportsMap Map of the aliases of all the external modules, keys are the module, values are the output filename
 * @returns {Object} The configured plugin
 */
function EntryPointMapperPlugin(exportsMap) {
	return {
		name: 'map-externals',
		setup(build) {
			let resolvedMap = {};
			let externalsList = build.initialOptions.entryPoints;

			build.onStart(async () => {
				for (let i = 0; i < externalsList.length; i++) {
					const input = externalsList[i];
					const output = await build.resolve(input, { resolveDir: "node_modules/" });
					const outputrel = path.relative(".", output.path).replace(/\\/g, "/");
					resolvedMap[outputrel] = input;
				}
			});

			build.onEnd((result) => {
				Object.entries(result.metafile.outputs).forEach(element => {
					if (element[1].entryPoint)
						exportsMap.set(
							resolvedMap[element[1].entryPoint],
							path.basename(element[0])
						);
				});
			});
		}
	};
}

/**
 * Inserts a text in the previous line of a target string
 * @param {string} source The text to be modified
 * @param {string} target The place to insert before
 * @param {string} text The text to insert
 * @returns {string} the modified string
 */
function stringInsertBefore(source, target, text)
{
	let ix = source.indexOf(target);
	if (ix === -1)
		return source;
	while(source[ix-1] === ' ' || source[ix-1] === '\t') ix--;
	let arr = [
		source.substring(0, ix),
		text,
		source.substring(ix)
	];
	return arr.join('');
}

//-----------------------------------------
// Asynchronous build process
//-----------------------------------------
(async () => {

	console.log(process.memoryUsage());

	//-----------------------------------------
	// Build all the vendor modules
	//-----------------------------------------
	let exportsMap = new Map();

	let result = await build({
		define: {
			__VUE_OPTIONS_API__: true,
			__VUE_PROD_DEVTOOLS__: false
		},
		entryPoints: external,
		entryNames: '[name]-[hash]',
		bundle: true,
		splitting: true,
		minify: true,
		metafile: true,
		format: 'esm',
		//outfile: 'esdist/vendor.js',
		outdir: 'esdist/vendor',
		plugins: [
			timePlugin,
			EntryPointMapperPlugin(exportsMap)
		],
		conditions: ['module'],
		target: [
			'es2016',
		],
	});

	fs.writeFileSync('esvendors_meta.json', JSON.stringify(result.metafile, null, '\t'));
	let text = await analyzeMetafile(result.metafile, {
		verbose: true,
	})
	fs.writeFileSync('esvendors_metrics.json', text);

	console.log(exportsMap);

	//-----------------------------------------
	// Build all the src modules
	//-----------------------------------------
	let sourcePath = path.resolve(__dirname, 'src');

	result = await build({
		entryPoints: ['src/main.js'],
		bundle: true,
		minify: false,
		splitting: true,
		metafile: true,
		format: 'esm',
		outdir: 'esdist',
		external,
		loader: {
			'.png': 'file',
			'.ico': 'file',
			'.svg': 'file',
			'.gif': 'file',
			'.eot': 'file',
			'.ttf': 'file',
			'.woff': 'file',
			'.woff2': 'file',
		},
		assetNames: 'assets/[name].[hash]',
		plugins: [
			pluginVue(),
			sassPlugin({
				precompile(source, pathname) {
					var res = source.replace(/\~@\//g, sourcePath.replace(/\\/g, "/") + "/");
					return res;
				},
				//importMapper: (path) => path.replace(/^\~@\//, './src/')
			}),
			alias({ '@': sourcePath }),
			EsmExternalsPlugin(exportsMap, "./vendor/"),
			timePlugin,
		],
		target: [
			'es2016',
		],
	});

	//-----------------------------------------
	// Write up the collected metadata
	//-----------------------------------------
	fs.writeFileSync('esbuild_meta.json', JSON.stringify(result.metafile));

	text = await analyzeMetafile(result.metafile, {
		verbose: true,
	})
	fs.writeFileSync('esbuild_metrics.json', text);
	console.log(process.memoryUsage());

	//-----------------------------------------
	// Inject compiled entry points into index.html
	//-----------------------------------------
	let indexContent = fs.readFileSync("./public/index.html", {encoding: "utf8"}).trim();
	//Note: We could user 'cheerio' to parse the Html but the manipulation are too simple
	let changedDom = stringInsertBefore(
		indexContent,
		'</body>',
		'\t\t<script type="module" crossorigin src="main.js"></script>\r\n'
		);
	//console.log(changedDom);
	fs.writeFileSync('./esdist/index.html', changedDom);

	//-----------------------------------------
	// Copy all the public resources into the distribution folder
	//-----------------------------------------
	fs.copySync('./public', './esdist', {overwrite: false})
})();
