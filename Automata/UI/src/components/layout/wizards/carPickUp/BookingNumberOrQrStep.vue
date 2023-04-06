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
      @next="wizard.Goto(CarPickupWizard.EmailAdressStep)"
      :nextBtnDisabled="nextBtnDisabled"
    >
    </wizard-footer>
  </div>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';

export default {
  name: 'booking-number-or-qr-step',
  inject: ['wizard'],
  data() {
    return {
      text: '',
      CarPickupWizard,
    };
  },
  mounted() {},
  created() {},
  methods: {},
  computed: {
    nextBtnDisabled() {
      return !this.text;
    },
  },
  watch: {},
  components: {},
};
</script>
