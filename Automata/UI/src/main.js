import Vue from 'vue';
import './registerServiceWorker';
import vuetify from './plugins/vuetify';
import router from './router';
import './plugins';
import './filters';
import './components';
import App from './App.vue';
import { i18n } from './plugins/i18n';
import '@/styles/main.scss';
import { settings } from '@/settings';
import { AppFunctions } from './functions/AppFunctions';
Vue.config.productionTip = false;

let app = new Vue({
  vuetify,
  router,
  i18n,
  render: (h) => h(App),
});
app.$mount('#app');
// app.$vuetify.theme.dark = true;
let buildTime = process.env.VUE_APP_BUILD_TIME;
console.log(`UI build: ${new Date(buildTime).toLocaleString()}`);

if (settings.isProd) {
  AppFunctions.InitProduction();
}
