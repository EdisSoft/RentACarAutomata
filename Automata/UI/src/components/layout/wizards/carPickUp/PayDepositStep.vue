<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-simple-input>
        <template #title>
          {{ $t('wizards.carPickUp.payDeposit.title') }}
        </template>
      </wizard-simple-input>
    </v-sheet>
    <wizard-footer @next="Next()" :loading="isLetetZarolasLoading">
    </wizard-footer>
  </div>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';
import { SuccesResponse } from '@/enums/SuccesResponse';
import { AutoberlesService } from '@/services/AutoberlesService';
import { PosService } from '@/services/PosService';
import { useApi } from '@/utils/useApi';
import { computed, inject, ref } from 'vue';

export default {
  name: 'pay-deposit-step',
  setup() {
    let wizard = inject('wizard');
    let [isLetetZarolasLoading, LetetZarolas] = useApi(() => {
      return AutoberlesService.LetetZarolas(wizard.form.Reservation.Id);
    });

    let Next = async () => {
      let [success, data] = await LetetZarolas();
      if (!success) {
        return;
      }
      if (data.Id == SuccesResponse.Next) {
        wizard.Goto(CarPickupWizard.PayRentalFeeStep);
      }
    };
    PosService.LetetZarolas();
    return {
      isLetetZarolasLoading,
      Next,
    };
  },
};
</script>
<style scoped></style>
