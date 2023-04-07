<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-simple-input>
        <template #title>
          {{ $t('wizards.carPickUp.driverLicense.title') }}
        </template>
      </wizard-simple-input>
    </v-sheet>
    <wizard-footer
      @next="Next()"
      @back="Back()"
      :nextBtnText="nextBtnText"
      :loading="isScanLicenceFrontLoading || isScanLicenceBackLoading"
      :back-btn="[front, back].some((s) => s)"
    >
    </wizard-footer>
  </div>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';
import { SuccesResponse } from '@/enums/SuccesResponse';
import { i18n } from '@/plugins/i18n';
import { AutoberlesService } from '@/services/AutoberlesService';
import { useApi } from '@/utils/useApi';
import { computed, inject, ref } from 'vue';

export default {
  name: 'driving-license-step',
  setup() {
    let wizard = inject('wizard');
    let front = ref(null);
    let back = ref(null);
    let [isScanLicenceFrontLoading, ScanLicenceFront] = useApi(() => {
      return AutoberlesService.ScanLicenceFront(wizard.form.Reservation.Id);
    });
    let [isScanLicenceBackLoading, ScanLicenceBack] = useApi(() => {
      return AutoberlesService.ScanLicenceBack(wizard.form.Reservation.Id);
    });

    let Next = async () => {
      if (!front.value) {
        let [success, data] = await ScanLicenceFront();
        if (!success) {
          return;
        }
        if (data.Id == SuccesResponse.Next) {
          front.value = data;
        }
        return;
      }
      if (!back.value) {
        let [success, data] = await ScanLicenceBack();
        if (!success) {
          return;
        }
        if (data.Id == SuccesResponse.Next) {
          wizard.Goto(CarPickupWizard.IdCardStep);
        }
        return;
      }
    };
    let Back = () => {
      if (back.value) {
        back.value = null;
        return;
      }
      if (front.value) {
        front.value = null;
        return;
      }
    };
    let nextBtnText = computed(() => {
      if (!front.value) {
        return i18n.t('common.scanFront');
      }
      if (!back.value) {
        return i18n.t('common.scanBack');
      }
      return null;
    });
    return {
      isScanLicenceBackLoading,
      isScanLicenceFrontLoading,
      Next,
      Back,
      CarPickupWizard,
      nextBtnText,
      front,
      back,
    };
  },
};
</script>
<style scoped></style>
