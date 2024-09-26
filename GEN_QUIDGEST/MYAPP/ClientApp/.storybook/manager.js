import { addons } from '@storybook/manager-api';
//import yourTheme from './YourTheme';
import { create } from '@storybook/theming/create';

const myTheme = create({
    base: 'light',
    // Typography
    //fontBase: '"Open Sans", sans-serif',
    //fontCode: 'monospace',
  
    brandTitle: 'Genio Proto',
    brandUrl: 'https://genio.quidgest.com',
    brandImage: 'genio-dark.png',
    brandTarget: '_self',
  /*
    //
    colorPrimary: '#3A10E5',
    colorSecondary: '#585C6D',
  
    // UI
    appBg: '#ffffff',
    appContentBg: '#ffffff',
    appBorderColor: '#585C6D',
    appBorderRadius: 4,
  
    // Text colors
    textColor: '#10162F',
    textInverseColor: '#ffffff',
  
    // Toolbar default and active colors
    barTextColor: '#9E9E9E',
    barSelectedColor: '#585C6D',
    barBg: '#ffffff',
  
    // Form colors
    inputBg: '#ffffff',
    inputBorder: '#10162F',
    inputTextColor: '#10162F',
    inputBorderRadius: 2,*/
  });

addons.setConfig({
  theme: myTheme,
});