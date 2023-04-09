import { i18n } from '@/plugins/i18n';

export function vRequired(msg) {
  return (v) => {
    if (v === 0) {
      return true;
    }
    return v === true || !!v || msg || i18n.t('validations.required');
  };
}

export function vMinLength(n, msg) {
  return (v) => {
    return (
      v.length >= n || msg || i18n.t('validations.minLength', { length: n })
    );
  };
}

export function vEmail(msg) {
  return (v) => {
    const re =
      /^([a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?)$/;
    return (
      re.test(String(v).toLowerCase()) || msg || i18n.t('validations.email')
    );
  };
}
