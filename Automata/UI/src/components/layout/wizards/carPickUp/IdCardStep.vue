<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-simple-input>
        <template #title>
          {{ $t('wizards.carPickUp.idCardOrPassport.title') }}
        </template>
      </wizard-simple-input>
    </v-sheet>
    <wizard-footer
      @next="Next()"
      @back="Back()"
      :nextBtnText="nextBtnText"
      :loading="isScanIdCardFrontOrPassportLoading || isScanIdCardBackLoading"
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
  name: 'id-card-step',
  setup() {
    let wizard = inject('wizard');
    let front = ref(null);
    let back = ref(null);
    let [isScanIdCardFrontOrPassportLoading, ScanIdCardFrontOrPassport] =
      useApi(() => {
        return AutoberlesService.ScanIdCardFrontOrPassport(
          wizard.form.Reservation?.Id
        );
      });
    let [isScanIdCardBackLoading, ScanIdCardBack] = useApi(() => {
      return AutoberlesService.ScanIdCardBack(wizard.form.Reservation?.Id);
    });

    let Next = async () => {
      if (!front.value) {
        let [success, data] = await ScanIdCardFrontOrPassport();
        if (!success) {
          return;
        }
        if (data.Id == SuccesResponse.Next) {
          front.value = data;
        }
        if (data.Id == SuccesResponse.Skip) {
          wizard.Goto(CarPickupWizard.CreditCardStep);
        }
        return;
      }
      if (!back.value) {
        let [success, data] = await ScanIdCardBack();
        if (!success) {
          return;
        }
        if (data.Id == SuccesResponse.Next) {
          wizard.Goto(CarPickupWizard.CreditCardStep);
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
      isScanIdCardFrontOrPassportLoading,
      isScanIdCardBackLoading,
      Next,
      Back,
      CarPickupWizard,
      nextBtnText,
      front,
      back,
    };
  },
  data() {
    return {};
  },
  mounted() {},
  created() {},
  methods: {},
  computed: {},
  watch: {},
  components: {},
};
</script>
<style scoped></style>
