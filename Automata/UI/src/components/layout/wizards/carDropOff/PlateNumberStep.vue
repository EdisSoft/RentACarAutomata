<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-text-input v-model="input" :maxlength="15">
        <template #title>
          {{ $t('wizards.carDropOff.plateNumber.title') }}
        </template>
      </wizard-text-input>
    </v-sheet>
    <wizard-footer
      :nextBtnDisabled="nextBtnDisabled"
      @next="Next()"
      :loading="isGetFoglalasLoading"
    >
    </wizard-footer>
  </div>
</template>

<script>
import { CarDropOffWizard } from '@/enums/CarDropOffWizard';
import { SuccesResponse } from '@/enums/SuccesResponse';
import { AutoleadasService } from '@/services/AutoleadasService';
import { useApi } from '@/utils/useApi';
import { computed, inject, ref } from 'vue';

export default {
  name: 'plate-number-step',
  setup() {
    let wizard = inject('wizard');
    let input = ref('');
    let [isGetFoglalasLoading, GetFoglalas] = useApi(() => {
      return AutoleadasService.GetFoglalas(input.value);
    });
    let nextBtnDisabled = computed(() => {
      let text = input.value.trim();
      return text.length < 3;
    });
    let Next = async () => {
      let [success, reservation] = await GetFoglalas();
      if (!success) {
        return;
      }
      wizard.SetFormValue('Reservation', reservation);
      wizard.Goto(CarDropOffWizard.TaxiService);
    };
    return {
      isGetFoglalasLoading,
      input,
      nextBtnDisabled,
      Next,
    };
  },
};
</script>
