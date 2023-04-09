<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <v-form class="fill-height" ref="form">
        <wizard-text-input v-model="email" :rules="validations.email">
          <template #title>
            {{ $t('wizards.carPickUp.emailAdress.title') }}
          </template>
        </wizard-text-input>
      </v-form>
    </v-sheet>
    <wizard-footer @next="Next()" :loading="isSaveEmailLoading">
    </wizard-footer>
  </div>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';
import { AutoberlesService } from '@/services/AutoberlesService';
import { useApi } from '@/utils/useApi';
import { SuccesResponse } from '@/enums/SuccesResponse.js';
import { computed, inject, ref } from 'vue';
import { vEmail, vRequired } from '@/utils/vuetifyFormRules';

export default {
  name: 'email-adress-step',
  setup() {
    let wizard = inject('wizard');
    let email = ref('');
    let form = ref(null);
    let [isSaveEmailLoading, SaveEmail] = useApi(() => {
      return AutoberlesService.SaveEmail(
        wizard.form.Reservation.Id,
        email.value
      );
    });
    let Next = async () => {
      let isValid = form.value.validate();
      if (!isValid) {
        return;
      }
      let [success, data] = await SaveEmail();
      if (!success) {
        return;
      }
      if (data.Id == SuccesResponse.Next) {
        wizard.Goto(CarPickupWizard.SignStep);
      }
    };
    let validations = {
      email: [vRequired(), vEmail()],
    };
    return {
      isSaveEmailLoading,
      email,
      form,
      validations,
      Next,
      CarPickupWizard,
    };
  },
};
</script>
