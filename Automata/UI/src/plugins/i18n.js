import { LanguageList } from '@/data/languages';
import { settings } from '@/settings';
import Vue from 'vue';
import VueI18n from 'vue-i18n';

Vue.use(VueI18n);

let dicts = {};
let localeStorageKey = settings.appName + '-language';

LanguageList.forEach((f) => {
  if (f.dict) {
    dicts[f.key] = f.dict;
  }
});

export const i18n = new VueI18n({
  locale: localStorage.getItem(localeStorageKey) || 'en',
  fallbackLocale: 'en',
  messages: dicts,
});

export const SetLocale = (key) => {
  i18n.locale = key;
  localStorage.setItem(localeStorageKey, key);
};
