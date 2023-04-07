import vuetify from '@/plugins/vuetify';
import { CreateVuetifyConfrimDialogHandler } from '@/utils/vuetifyConfirmDialog';
import ConfirmModal from '@/components/modals/base/ConfirmModal.vue';
import { i18n } from '@/plugins/i18n';
import WarningLevel from '@/enums/WaningLevel';

let createConfirmModal = CreateVuetifyConfrimDialogHandler(
  vuetify,
  i18n,
  ConfirmModal
);

class NotificationFunctions {
  AjaxError(error) {
    console.error(error);
    let title = 'Error';
    let text = 'Unexpected error';
    let warningLevel = WarningLevel.Error;

    if (error && error.isAxiosError && error.response) {
      let responseData = error.response.data;
      title = responseData.title || title;
      text = responseData.message || text;
      warningLevel = error.level;
    }
    let buttonTrueColor = undefined;
    switch (warningLevel) {
      case WarningLevel.Error:
        buttonTrueColor = 'error';
        break;
      case WarningLevel.Warning:
      case WarningLevel.Validation:
        buttonTrueColor = 'warning';
        break;
      default:
        buttonTrueColor = 'error';
        break;
    }
    return createConfirmModal(title, text, {
      buttonTrueColor,
      buttonFalse: false,
      buttonTrueText: i18n.t('common.ok'),
    });
  }
  ConfirmDialog(title, text, options) {
    return createConfirmModal(title, text, { ...options });
  }
}

let instance = new NotificationFunctions();
export { instance as NotificationFunctions };
