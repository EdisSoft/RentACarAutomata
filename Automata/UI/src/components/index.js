import Vue from 'vue';
import upperFirst from 'lodash/upperFirst';
import camelCase from 'lodash/camelCase';
import kebabCase from 'lodash/kebabCase';

const requireComponents = require.context(
  '.',
  true,
  /^((?!Lazy\.vue).)*\.vue$/
);
requireComponents.keys().forEach((fileName) => {
  const componentConfig = requireComponents(fileName);
  const componentToRegister = componentConfig.default || componentConfig;
  var componentName = upperFirst(camelCase(componentToRegister.name));
  if (componentName) {
    Vue.component(componentName, componentToRegister);
  } else {
    console.error('Nincs komponens név: ', fileName);
  }
});

const requireComponentsLazy = require.context('.', true, /Lazy\.vue$/, 'lazy');

requireComponentsLazy.keys().forEach((fileName) => {
  var componentName = kebabCase(fileName.split('/').pop().split('.')[0]);

  if (componentName) {
    Vue.component(componentName, () => import(`${fileName}`));
  } else {
    console.error('Nincs komponens név: ', fileName);
  }
});
