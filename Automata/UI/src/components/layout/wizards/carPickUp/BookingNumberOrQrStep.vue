<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <v-form class="fill-height" ref="form">
        <wizard-text-input v-model="text" :rules="validations.text">
          <template #title>
            {{ $t('wizards.carPickUp.bookingNumberOrQr.title') }}
          </template>
        </wizard-text-input>
      </v-form>
    </v-sheet>
    <wizard-footer @next="Next()" :loading="isFoglalasokLoading">
    </wizard-footer>
  </div>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';
import { AutoberlesService } from '@/services/AutoberlesService';
import { useApi } from '@/utils/useApi';
import { useInterval } from '@vueuse/shared';
import { WizardFunctions } from '@/functions/WizardFunctions';
import { inject } from 'vue';
import { settings } from '@/settings';
import { vMinLength, vRequired } from '@/utils/vuetifyFormRules';
import { QrCodeService } from '@/services/QrCodeService';

export default {
  name: 'booking-number-or-qr-step',
  data() {
    return {
      text: '',
      CarPickupWizard,
    };
  },
  setup() {
    let wizard = inject('wizard');
    // let a = useApi(() => {
    //   return AutoberlesService.GetFoglalasok();
    // });
    let [isFoglalasokLoading, Getfoglalasok] = useApi(() => {
      return AutoberlesService.GetFoglalasok();
    });
    let [isQrLoading, IsQrCode] = useApi(
      () => {
        return AutoberlesService.IsQrCode();
      },
      { silentError: true }
    );
    let CheckQr = async () => {
      if (isQrLoading.value || isFoglalasokLoading.value) {
        return;
      }
      let [success, reservation] = await IsQrCode();
      if (!success) {
        return;
      }
      if (!reservation) {
        return;
      }
      WizardFunctions.HandleNavigationForReservation(wizard, reservation);
    };
    if (settings.isProd) {
      useInterval(3000, { immediate: true, callback: CheckQr });
    }
    QrCodeService.Start();
    return { isFoglalasokLoading, Getfoglalasok, wizard };
  },
  created() {},
  methods: {
    async Next() {
      let isValid = this.$refs.form.validate();
      if (!isValid) {
        return;
      }
      let [success, data] = await this.Getfoglalasok();
      if (!success) {
        return;
      }
      this.wizard.SetFormValue('Reservations', data);
      if (data.length == 1) {
        let reservation = data[0];
        WizardFunctions.HandleNavigationForReservation(
          this.wizard,
          reservation
        );
      } else {
        this.wizard.Goto(CarPickupWizard.ChooseReservation);
      }
    },
  },
  computed: {
    validations() {
      return {
        text: [vRequired(), vMinLength(3)],
      };
    },
  },
  watch: {},
  components: {},
};
</script>
