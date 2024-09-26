/** @type { import('@storybook/vue3-vite').StorybookConfig } */

const config = {
  stories: [
    "../tests/docs/**/*.mdx",
    "../tests/cases/**/*.stories.@(js|jsx|mjs|ts|tsx)"
  ],
  addons: [
    "@storybook/addon-links",
    "@storybook/addon-essentials",
    "@storybook/addon-interactions",
  ],
  framework: {
    name: "@storybook/vue3-vite",
    options: {},
  },
  docs: {
    autodocs: "tag",
  },
  staticDirs: ['../tests/public']
}

export default config
