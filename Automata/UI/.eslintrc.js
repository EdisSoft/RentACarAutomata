module.exports = {
  root: true,
  env: {
    node: true,
  },
  extends: [
    'plugin:vue/essential',
    'eslint:recommended',
    'plugin:prettier/recommended',
  ],
  parserOptions: {
    parser: '@babel/eslint-parser',
  },
  rules: {
    'require-await': 0,
    'no-console': 'off',
    'no-debugger': process.env.NODE_ENV === 'production' ? 'error' : 'warn',
    semi: [2, 'always'],
    'comma-dangle': 0,
    quotes: 0,
    'linebreak-style': 0,
    'no-unused-vars': 0,
    endOfLine: 0,
    'eol-last': 0,
    'no-throw-literal': 0,
    'no-unreachable': 0,
    eqeqeq: 0,
    indent: 0,
    'vue/attributes-order': 0,
    'import/no-named-as-default': 0,
    'vue/component-definition-name-casing': 0,
    'vue/no-v-html': 0,
    'vue/multi-word-component-names': 0,
    'no-undef': 'error',
  },
};
