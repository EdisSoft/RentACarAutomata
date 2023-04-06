import HU from 'country-flag-icons/string/1x1/HU';
import EN from 'country-flag-icons/string/1x1/GB';
import ES from 'country-flag-icons/string/1x1/ES';

let defaultKeyboard = {
  default: [
    '0 1 2 3 4 5 6 7 8 9 0 {backspace}',
    'q w e r t z u i o p',
    'a s d f g h j k l',
    '{lock} y x c v b n m , . -',
    '{spec} .com @ {space}',
  ],
  shift: [
    '0 1 2 3 4 5 6 7 8 9 0 {backspace}',
    'Q W E R T Z U I O P',
    'A S D F G H J K L',
    '{lock} Y X C V B N M , . -',
    '{spec} .com @ {space}',
  ],
  spec: [
    '0 1 2 3 4 5 6 7 8 9 0 {backspace}',
    '+ - x ÷ = / ( ) < > [ ] { }',
    '! @ # € % ^ & * \' " : ;',
    '{lock} , ? ` ~ \\ | _',
    '{spec} .com @ {space}',
  ],
};

export const Languages = Object.freeze({
  HU: {
    key: 'hu',
    svg: HU,
    keyboard: {
      default: [
        '0 1 2 3 4 5 6 7 8 9 0 ö ü ó {backspace}',
        'q w e r t z u i o p ő ú',
        'a s d f g h j k l é á ű',
        '{lock} í y x c v b n m , . -',
        '{spec} .hu .com @ {space}',
      ],
      shift: [
        '0 1 2 3 4 5 6 7 8 9 0 Ö Ü Ó {backspace}',
        'Q W E R T Z U I O P Ő Ú',
        'A S D F G H J K L É Á Ű',
        '{lock} Í Y X C V B N M , . -',
        '{spec} .hu .com @ {space}',
      ],
      spec: [
        '0 1 2 3 4 5 6 7 8 9 0 {backspace}',
        '+ x ÷ = / _ ( ) < > [ ] { }',
        '? ! @ # € % ^ & * - \' " : ;',
        '{lock} , ? ` ~ \\ |',
        '{spec} .hu .com @ {space}',
      ],
    },
  },
  EN: {
    key: 'en',
    svg: EN,
    keyboard: {
      ...defaultKeyboard,
    },
  },
  ES: {
    key: 'es',
    svg: ES,
    keyboard: {
      ...defaultKeyboard,
    },
  },
});
export const LanguageList = Array.from(Object.values(Languages));
