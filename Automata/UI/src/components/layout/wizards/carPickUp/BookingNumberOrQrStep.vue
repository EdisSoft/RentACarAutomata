<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-text-input v-model="text">
        <template #title>
          {{ $t('wizards.carPickUp.bookingNumberOrQr.title') }}
        </template>
      </wizard-text-input>
    </v-sheet>
    <wizard-footer
      @next="Next()"
      :nextBtnDisabled="nextBtnDisabled"
      :loading="isFoglalasokLoading"
    >
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
    let [isFoglalasokLoading, Getfoglalasok] = useApi(() => {
      return AutoberlesService.GetFoglalasok();
    });
    let [isQrLoading, IsQrCode] = useApi(() => {
      return AutoberlesService.IsQrCode();
    });
    let CheckQr = async () => {
      if (isQrLoading.value || isFoglalasokLoading.value) {
        return;
      }
      let [success, reservation] = await IsQrCode();
      if (!success) {
        return;
      }
      WizardFunctions.HandleNavigationForReservation(wizard, reservation);
    };
    if (settings.isProd) {
      useInterval(3000, { immediate: true, callback: CheckQr });
    }
    return { isFoglalasokLoading, Getfoglalasok, wizard };
  },

  created() {},
  methods: {
    async Next() {
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
    nextBtnDisabled() {
      let text = this.text.trim();
      return text.length < 3;
    },
  },
  watch: {},
  components: {},
};
</script>
