import Vue from 'vue';

export function CreateVuetifyConfrimDialogHandler(vuetify, i18n, component) {
  const Ctor = Vue.extend(Object.assign({ vuetify, i18n }, component));
  function createDialog(options) {
    const container = document.body;
    return new Promise((resolve) => {
      const cmp = new Ctor(
        Object.assign(
          {},
          {
            propsData: Object.assign({}, options),
            destroyed: () => {
              container.removeChild(cmp.$el);
              resolve(cmp.value);
            },
          }
        )
      );
      container.appendChild(cmp.$mount().$el);
    });
  }

  function showConfirmDialog(title, message, options = {}) {
    options.title = title;
    options.message = message;
    return createDialog(options);
  }
  return showConfirmDialog;
}
