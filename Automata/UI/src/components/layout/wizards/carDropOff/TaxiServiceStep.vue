<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-select-input
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
    </v-sheet>
    <wizard-footer @next="Next()" :nextBtnDisabled="nextBtnDisabled">
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

    let nextBtnDisabled = computed(() => {
      return selectedItem.value == null;
    });
    let Next = async () => {
      if (selectedItem.value.id == 1) {
        wizard.SetFormValue('TaxiRendeles', true);
        wizard.Goto(CarDropOffWizard.InsertCarKey);
      } else {
        wizard.SetFormValue('TaxiRendeles', false);
        wizard.Goto(CarDropOffWizard.InsertCarKey);
      }
    };
    return {
      selectedItem,
      nextBtnDisabled,
      Next,
    };
  },
};
</script>
<style scoped></style>
