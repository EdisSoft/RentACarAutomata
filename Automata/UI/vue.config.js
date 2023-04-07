const { defineConfig } = require('@vue/cli-service');

process.env.VUE_APP_BUILD_TIME = new Date().toISOString();

module.exports = defineConfig({
  transpileDependencies: ['vuetify'],
  chainWebpack: (config) => {
    config.module
      .rule('pdf')
      .test(/\.(pdf)(\?.*)?$/)
      .use('file-loader')
      .loader('file-loader')
      .options({
        name: 'assets/pdf/[name].[hash:8].[ext]',
      });
  },
});
