import Vue from 'vue';
import VueRouter from 'vue-router';
import HomeView from '../views/HomeView.vue';
import WelcomeView from '../views/WelcomeView.vue';

Vue.use(VueRouter);

const routes = [
  {
    path: '/',
    name: 'welcome',
    component: WelcomeView,
  },
  {
    path: '/home',
    name: 'home',
    component: HomeView,
  },
  {
    path: '/car-pick-up',
    name: 'car-pick-up',
    component: () =>
      import(
        /* webpackChunkName: "car-pick-up" */ '../views/CarPickUpView.vue'
      ),
  },
  {
    path: '/car-drop-off',
    name: 'car-drop-off',
    component: () =>
      import(
        /* webpackChunkName: "car-drop-off" */ '../views/CarDropOffView.vue'
      ),
  },
  {
    path: '/about',
    name: 'about',
    component: () =>
      import(/* webpackChunkName: "about" */ '../views/AboutView.vue'),
  },
  {
    path: '/input',
    name: 'input',
    component: () =>
      import(/* webpackChunkName: "input" */ '../views/InputView.vue'),
  },
  {
    path: '/info',
    name: 'info',
    component: () =>
      import(/* webpackChunkName: "info" */ '../views/InfoView.vue'),
  },
  {
    path: '/sign',
    name: 'sign',
    component: () =>
      import(/* webpackChunkName: "sign" */ '../views/SignView.vue'),
  },
];

const router = new VueRouter({
  routes,
});

export default router;
