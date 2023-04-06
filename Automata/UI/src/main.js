import Vue from 'vue';
import './registerServiceWorker';
import vuetify from './plugins/vuetify';
import router from './router';
import './plugins';
import './components';
import App from './App.vue';
import { i18n } from './plugins/i18n';
import '@/styles/main.scss';
Vue.config.productionTip = false;

let app = new Vue({
  vuetify,
  router,
  i18n,
  render: (h) => h(App),
});
app.$mount('#app');
