import { readdirSync, readFileSync, writeFileSync } from 'fs'
import { extname, join, parse } from 'path'
import svgstore from 'svgstore'

/**
 * Specific bundling setup for this project.
 */
function ProjectPack()
{
	PackSvg('./public/Content/svg/', './public/Content/svgbundle.svg')
}

/**
 * Bundles all the svg files found in a souce directory into an single svg output file.
 * @param {string} dirname - The path to the directory containing all the svgs to be bundled
 * @param {string} output - The full filename of the desired output bundle
 */
function PackSvg(dirname, output)
{
	let sprites = svgstore()
	const files = readdirSync(dirname)

	files.forEach((file) => {
		if (extname(file) === '.svg')
		{
			let id = parse(file).name
			let content = readFileSync(join(dirname, file), 'utf8')
			sprites.add(id, content)
		}
	})

	writeFileSync(output, sprites.toString())
}

export default ProjectPack
