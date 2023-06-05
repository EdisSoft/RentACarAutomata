<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <v-form class="fill-height" ref="form">
        <wizard-text-input
          v-model="input"
          :maxlength="15"
          :rules="validations.input"
        >
          <template #title>
            {{ $t('wizards.carDropOff.plateNumber.title') }}
          </template>
        </wizard-text-input>
      </v-form>
    </v-sheet>
    <wizard-footer @next="Next()" :loading="isLeadasLoading"> </wizard-footer>
  </div>
</template>

<script>
import { CarDropOffWizard } from '@/enums/CarDropOffWizard';
import { AutoleadasService } from '@/services/AutoleadasService';
import { useApi } from '@/utils/useApi';
import { vMinLength, vRequired } from '@/utils/vuetifyFormRules';
import { inject, ref } from 'vue';

export default {
  name: 'plate-number-step',
  setup() {
    let wizard = inject('wizard');
    let input = ref('');
    let form = ref(null);
    let [isLeadasLoading, Leadas] = useApi(() => {
      return AutoleadasService.Leadas(input.value);
    });

    let Next = async () => {
      let isValid = form.value.validate();
      if (!isValid) {
        return;
      }
      let [success, reservation] = await Leadas();
      if (!success) {
        return;
      }
      wizard.SetFormValue('Reservation', reservation);
      wizard.Goto(CarDropOffWizard.TaxiService);
    };
    let validations = {
      input: [vRequired(), vMinLength(6)],
    };
    return {
      isLeadasLoading,
      input,
      form,
      validations,
      Next,
    };
  },
};
</script>
