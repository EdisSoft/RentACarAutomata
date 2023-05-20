<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-simple-input>
        <template #title>
          {{ $t('wizards.carPickUp.creditCard.title') }}
        </template>
      </wizard-simple-input>
    </v-sheet>
    <wizard-footer
      @next="Next()"
      @back="Back()"
      :nextBtnText="nextBtnText"
      :loading="isScanCreditCardFrontLoading || isScanCreditCardBackLoading"
      :back-btn="[front, back].some((s) => s)"
    >
    </wizard-footer>
  </div>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';
import { SuccesResponse } from '@/enums/SuccesResponse';
import { WizardFunctions } from '@/functions/WizardFunctions';
import { i18n } from '@/plugins/i18n';
import { AutoberlesService } from '@/services/AutoberlesService';
import { useApi } from '@/utils/useApi';
import { computed, inject, ref } from 'vue';

export default {
  name: 'credit-card-step',
  setup() {
    let wizard = inject('wizard');
    let front = ref(null);
    let back = ref(null);
    let [isScanCreditCardFrontLoading, ScanCreditCardFront] = useApi(() => {
      return AutoberlesService.ScanCreditCardFront(wizard.form.Reservation.Id);
    });
    let [isScanCreditCardBackLoading, ScanCreditCardBack] = useApi(() => {
      return AutoberlesService.ScanCreditCardBack(wizard.form.Reservation.Id);
    });

    let Next = async () => {
      if (!front.value) {
        let [success, data] = await ScanCreditCardFront();
        if (!success) {
          return;
        }
        if (data.Id == SuccesResponse.Next) {
          front.value = data;
        }
        return;
      }
      if (!back.value) {
        let [success, data] = await ScanCreditCardBack();
        if (!success) {
          return;
        }
        if (data.Id == SuccesResponse.Next) {
          WizardFunctions.HandleNavigationForPayment(
            wizard,
            wizard.form.Reservation,
            CarPickupWizard.PayDepositStep
          );
        }
        return;
      }
    };
    let Back = () => {
      if (back.value) {
        back.value = null;
        return;
      }
      if (front.value) {
        front.value = null;
        return;
      }
    };
    let nextBtnText = computed(() => {
      if (!front.value) {
        return i18n.t('common.scanFront');
      }
      if (!back.value) {
        return i18n.t('common.scanBack');
      }
      return null;
    });
    return {
      isScanCreditCardFrontLoading,
      isScanCreditCardBackLoading,
      Next,
      Back,
      CarPickupWizard,
      nextBtnText,
      front,
      back,
    };
  },
};
</script>
<style scoped></style>
