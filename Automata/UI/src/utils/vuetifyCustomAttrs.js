import Vue from 'vue';

export const vuetifyCustomTextField = Object.freeze({
  outlined: true,
  dense: true,
  flat: true,
  solo: false,
  height: 34,
  hideDetails: 'auto',
  class: 'custom-input custom-text-field',
  validateOnBlur: true,
});
export const vuetifyCustomTextArea = Object.freeze({
  outlined: true,
  dense: true,
  flat: true,
  rows: 3,
  hideDetails: 'auto',
  class: 'custom-input custom-text-area',
});
export const vuetifyCustomSelect = Object.freeze({
  itemValue: 'Id',
  itemText: 'Nev',
  outlined: true,
  dense: true,
  flat: true,
  hideDetails: 'auto',
  class: 'custom-input custom-select',
});
export const vuetifyCustomSlider = Object.freeze({});

Vue.prototype.$vuetifyCustomTextField = vuetifyCustomTextField;
Vue.prototype.$vuetifyCustomTextArea = vuetifyCustomTextArea;
Vue.prototype.$vuetifyCustomSelect = vuetifyCustomSelect;
Vue.prototype.$vuetifyCustomSlider = vuetifyCustomSlider;
