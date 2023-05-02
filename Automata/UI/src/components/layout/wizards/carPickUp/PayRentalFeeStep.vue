<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-simple-input>
        <template #title>
          {{ $t('wizards.carPickUp.payRentalFee.title') }}
        </template>
      </wizard-simple-input>
    </v-sheet>
    <wizard-footer @next="Next()" :loading="isBerletiDijFizetesLoading">
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
  name: 'pay-rental-fee-step',
  setup() {
    let wizard = inject('wizard');
    let [isBerletiDijFizetesLoading, BerletiDijFizetes] = useApi(() => {
      return AutoberlesService.BerletiDijFizetes(wizard.form.Reservation.Id);
    });

    let Next = async () => {
      let [success, data] = await BerletiDijFizetes();
      if (!success) {
        return;
      }
      if (data.Id == SuccesResponse.Next) {
        wizard.Goto(CarPickupWizard.FinalStep);
      }
    };
    PosService.Fizetes();
    return {
      isBerletiDijFizetesLoading,
      Next,
    };
  },
};
</script>
<style scoped></style>
