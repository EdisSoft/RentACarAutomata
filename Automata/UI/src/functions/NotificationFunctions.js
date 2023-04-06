import vuetify from '@/plugins/vuetify';
import { CreateVuetifyConfrimDialogHandler } from '@/utils/vuetifyConfirmDialog';
import ConfirmModal from '@/components/modals/base/ConfirmModal.vue';
import { i18n } from '@/plugins/i18n';

let createConfirmModal = CreateVuetifyConfrimDialogHandler(
  vuetify,
  i18n,
  ConfirmModal
);

class NotificationFunctions {
  ConfirmDialog(title, text, options) {
    return createConfirmModal(title, text, { ...options });
  }
}

let instance = new NotificationFunctions();
export { instance as NotificationFunctions };
