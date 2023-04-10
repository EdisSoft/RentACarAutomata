<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-sign-input v-model="points" ref="input">
        <template #title>
          {{ $t('wizards.carPickUp.sign.title') }}
        </template>
      </wizard-sign-input>
    </v-sheet>
    <wizard-footer
      :nextBtnDisabled="nextBtnDisabled"
      @next="Next()"
      :loading="isSaveAlairasLoading"
    >
    </wizard-footer>
  </div>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';
import { SuccesResponse } from '@/enums/SuccesResponse';
import { AutoberlesService } from '@/services/AutoberlesService';
import { useApi } from '@/utils/useApi';
import { computed, inject, ref } from 'vue';

export default {
  name: 'sign-step',

  setup() {
    let wizard = inject('wizard');
    let points = ref(0);
    let input = ref(null);
    let [isSaveAlairasLoading, SaveAlairas] = useApi(() => {
      let base64 = input.value.GetBase64Image();
      return AutoberlesService.SaveAlairas(wizard.form.Reservation.Id, base64);
    });
    let nextBtnDisabled = computed(() => {
      return points.value < 100;
    });
    let Next = async () => {
      let [success, data] = await SaveAlairas();
      if (!success) {
        return;
      }
      if (data.Id == SuccesResponse.Next) {
        wizard.Goto(CarPickupWizard.DrivingLicenseStep);
      }
    };

    return {
      isSaveAlairasLoading,
      points,
      nextBtnDisabled,
      Next,
      CarPickupWizard,
      input,
    };
  },
};
</script>
<style scoped></style>
