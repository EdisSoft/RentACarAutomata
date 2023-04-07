import { LanguageList } from '@/data/languages';

class AppFunctions {
  GetLanguage(locale) {
    return LanguageList.find((f) => f.key == locale);
  }
  InitProduction() {
    this.DisableContextMenu();
    this.DisableUserSelect();
  }
  DisableContextMenu() {
    document.addEventListener('contextmenu', (event) => event.preventDefault());
  }
  DisableUserSelect() {
    document.body.classList.add('disable-user-select');
  }
}

let instance = new AppFunctions();
export { instance as AppFunctions };
