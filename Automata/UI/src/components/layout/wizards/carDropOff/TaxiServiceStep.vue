<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-select-input
        v-if="!taxiRendelesConfirm"
        v-model="selectedItem"
        :options="[
          { id: 1, title: $t('common.yes') },
          { id: 2, title: $t('common.no') },
        ]"
      >
        <template #title>
          {{ $t('wizards.carDropOff.taxiService.title') }}
        </template>
      </wizard-select-input>
      <wizard-simple-input v-else :class="{ 'error--text': taxiRendelesError }">
        <template #title>
          {{ taxiRendelesConfirm }}
        </template>
      </wizard-simple-input>
    </v-sheet>
    <wizard-footer
      @next="Next()"
      :nextBtnDisabled="nextBtnDisabled"
      :loading="isTaxiRendelesLoading"
    >
    </wizard-footer>
  </div>
</template>

<script>
import { CarDropOffWizard } from '@/enums/CarDropOffWizard';
import { SuccesResponse } from '@/enums/SuccesResponse';
import { i18n } from '@/plugins/i18n';
import { AutoleadasService } from '@/services/AutoleadasService';
import { useApi } from '@/utils/useApi';
import { computed, inject, ref } from 'vue';

export default {
  name: 'taxi-service-step',
  setup() {
    let wizard = inject('wizard');
    let selectedItem = ref(null);
    let taxiRendelesConfirm = ref('');
    let taxiRendelesError = ref(false);
    let [isTaxiRendelesLoading, TaxiRendeles] = useApi(() => {
      return AutoleadasService.TaxiRendeles(wizard.form.Reservation.Id, true);
    });
    let nextBtnDisabled = computed(() => {
      return selectedItem.value == null;
    });
    let Next = async () => {
      if (taxiRendelesConfirm.value) {
        wizard.Goto(CarDropOffWizard.InsertCarKey);
        return;
      }
      if (selectedItem.value.id == 1) {
        let [success, data] = await TaxiRendeles();
        if (success && data.Id == SuccesResponse.Next) {
          taxiRendelesConfirm.value = i18n.t(
            'wizards.carDropOff.taxiService.succesText'
          );
        } else {
          taxiRendelesError.value = true;
          taxiRendelesConfirm.value = i18n.t(
            'wizards.carDropOff.taxiService.errorText'
          );
        }
        return;
      }
      wizard.Goto(CarDropOffWizard.InsertCarKey);
    };
    return {
      isTaxiRendelesLoading,
      selectedItem,
      nextBtnDisabled,
      Next,
      taxiRendelesConfirm,
      taxiRendelesError,
    };
  },
};
</script>
<style scoped></style>
