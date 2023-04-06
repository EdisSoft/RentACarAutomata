import { LanguageList } from '@/data/languages';

class AppFunctions {
  GetLanguage(locale) {
    return LanguageList.find((f) => f.key == locale);
  }
}

let instance = new AppFunctions();
export { instance as AppFunctions };
