<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-simple-input v-if="!hibasNyitas">
        <template #title>
          {{ $t('wizards.carDropOff.insertCarKey.title', { num: azon }) }}
        </template>
      </wizard-simple-input>
      <wizard-simple-input v-else>
        <template #title>
          {{ $t('wizards.carDropOff.taxiService.errorText') }}
        </template>
      </wizard-simple-input>
      <v-overlay
        :value="isUresRekeszNyitasLoading"
        absolute
        color="white"
        :opacity="1"
      >
        <v-progress-circular
          indeterminate
          color="primary"
          size="64"
        ></v-progress-circular>
      </v-overlay>
    </v-sheet>
    <wizard-footer
      @next="Next()"
      :loading="isKulcsLeadasLoading"
      :nextBtnDisabled="isUresRekeszNyitasLoading"
    >
    </wizard-footer>
  </div>
</template>

<script>
import { CarDropOffWizard } from '@/enums/CarDropOffWizard';
import { SuccesResponse } from '@/enums/SuccesResponse';
import { AutoleadasService } from '@/services/AutoleadasService';
import { useApi } from '@/utils/useApi';
import { computed, inject, onMounted, ref } from 'vue';

export default {
  name: 'insert-car-key-step',
  setup() {
    let wizard = inject('wizard');
    let input = ref('');
    let hibasNyitas = ref(false);
    let azon = ref(null);
    let [isUresRekeszNyitasLoading, UresRekeszNyitas] = useApi(() => {
      return AutoleadasService.UresRekeszNyitas(input.value);
    });
    let [isKulcsLeadasLoading, KulcsLeadas] = useApi(() => {
      return AutoleadasService.KulcsLeadas(
        input.value,
        wizard.form.TaxiRendeles
      );
    });
    let nextBtnDisabled = computed(() => {
      let text = input.value.trim();
      return text.length < 3;
    });
    let Next = async () => {
      let [success, result] = await KulcsLeadas();
      if (success && result.Id == SuccesResponse.Next) {
        wizard.Goto(CarDropOffWizard.Final);
      }
    };
    onMounted(async () => {
      let [success, data] = await UresRekeszNyitas();
      if (success && data.Id == SuccesResponse.Next) {
        azon.value = data.Text;
      } else {
        hibasNyitas.value = true;
      }
    });
    return {
      isUresRekeszNyitasLoading,
      isKulcsLeadasLoading,
      input,
      azon,
      hibasNyitas,
      nextBtnDisabled,
      Next,
    };
  },
};
</script>
<style scoped></style>
