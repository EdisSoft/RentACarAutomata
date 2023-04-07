import { LanguageList } from '@/data/languages';
import Vue from 'vue';
import VueI18n from 'vue-i18n';

Vue.use(VueI18n);

let dicts = {};
LanguageList.forEach((f) => {
  if (f.dict) {
    dicts[f.key] = f.dict;
  }
});

export const i18n = new VueI18n({
  locale: 'en',
  fallbackLocale: 'en',
  messages: dicts,
});
