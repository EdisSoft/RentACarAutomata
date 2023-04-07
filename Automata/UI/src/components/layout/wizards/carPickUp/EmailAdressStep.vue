<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-text-input v-model="email">
        <template #title>
          {{ $t('wizards.carPickUp.emailAdress.title') }}
        </template>
      </wizard-text-input>
    </v-sheet>
    <wizard-footer
      :nextBtnDisabled="nextBtnDisabled"
      @next="Next()"
      :loading="isSaveEmailLoading"
    >
    </wizard-footer>
  </div>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';
import { AutoberlesService } from '@/services/AutoberlesService';
import { useApi } from '@/utils/useApi';
import { SuccesResponse } from '@/enums/SuccesResponse.js';
import { computed, inject, ref } from 'vue';

export default {
  name: 'email-adress-step',
  setup() {
    let wizard = inject('wizard');
    let email = ref('');
    let [isSaveEmailLoading, SaveEmail] = useApi(() => {
      return AutoberlesService.SaveEmail(
        wizard.form.Reservation.Id,
        email.value
      );
    });
    let nextBtnDisabled = computed(() => {
      let text = email.value.trim();
      return text.length < 3 || !text.includes('@') || !text.includes('.');
    });
    let Next = async () => {
      let [success, data] = await SaveEmail();
      if (!success) {
        return;
      }
      if (data.Id == SuccesResponse.Next) {
        wizard.Goto(CarPickupWizard.SignStep);
      }
    };
    return {
      isSaveEmailLoading,
      email,
      nextBtnDisabled,
      Next,
      CarPickupWizard,
    };
  },
};
</script>
