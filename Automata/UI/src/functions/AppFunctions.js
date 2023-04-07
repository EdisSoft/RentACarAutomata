import { LanguageList } from '@/data/languages';

class AppFunctions {
  GetLanguage(locale) {
    return LanguageList.find((f) => f.key == locale);
  }
  InitProduction() {
    this.DisableContextMenu();
  }
  DisableContextMenu() {
    document.addEventListener('contextmenu', (event) => event.preventDefault());
  }
}

let instance = new AppFunctions();
export { instance as AppFunctions };
