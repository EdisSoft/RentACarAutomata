<template>
  <v-stepper v-model="step" class="wizard" eager>
    <v-stepper-header>
      <v-stepper-step
        :complete="step > CarPickupWizard.BookingNumberOrQr"
        :step="CarPickupWizard.BookingNumberOrQr"
      >
        {{ $t('wizards.carPickUp.bookingNumberOrQr.stepName') }}
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarPickupWizard.EmailAdressStep"
        :step="CarPickupWizard.EmailAdressStep"
      >
        {{ $t('wizards.carPickUp.emailAdress.stepName') }}
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarPickupWizard.SignStep"
        :step="CarPickupWizard.SignStep"
      >
        {{ $t('wizards.carPickUp.sign.stepName') }}
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarPickupWizard.DrivingLicenseStep"
        :step="CarPickupWizard.DrivingLicenseStep"
      >
        {{ $t('wizards.carPickUp.driverLicense.stepName') }}
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarPickupWizard.CreditCardStep"
        :step="CarPickupWizard.CreditCardStep"
      >
        {{ $t('wizards.carPickUp.creditCard.stepName') }}
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarPickupWizard.PaymentStep"
        :step="CarPickupWizard.PaymentStep"
      >
        {{ $t('wizards.carPickUp.payment.stepName') }}
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarPickupWizard.FinalStep"
        :step="CarPickupWizard.FinalStep"
      >
        {{ $t('wizards.carPickUp.final.stepName') }}
      </v-stepper-step>
    </v-stepper-header>
    <v-stepper-items>
      <v-stepper-content :step="CarPickupWizard.BookingNumberOrQr">
        <booking-number-or-qr-step></booking-number-or-qr-step>
      </v-stepper-content>
      <v-stepper-content :step="CarPickupWizard.EmailAdressStep">
        <email-adress-step></email-adress-step>
      </v-stepper-content>
      <v-stepper-content :step="CarPickupWizard.SignStep">
        <sign-step></sign-step>
      </v-stepper-content>
      <v-stepper-content :step="CarPickupWizard.DrivingLicenseStep">
        <driving-license-step></driving-license-step>
      </v-stepper-content>
      <v-stepper-content :step="CarPickupWizard.CreditCardStep">
        <credit-card-step></credit-card-step>
      </v-stepper-content>
      <v-stepper-content :step="CarPickupWizard.PaymentStep">
        <payment-step></payment-step>
      </v-stepper-content>
      <v-stepper-content :step="CarPickupWizard.FinalStep">
        <final-step></final-step>
      </v-stepper-content>
    </v-stepper-items>
  </v-stepper>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';
import router from '@/router';
import { useCountdown } from '@/utils/useCountdown';
import { useWizard } from '@/utils/useWizard';
import { computed } from 'vue';
export default {
  name: 'CarPickUpView',
  provide() {
    return {
      wizard: this,
      remaingTime: computed(() => {
        return this.remaingTime;
      }),
    };
  },
  setup(props, ctx) {
    const { Goto, step } = useWizard(CarPickupWizard.BookingNumberOrQr);
    const { remaingTime } = useCountdown(5 * 60 * 1000, () => {
      router.push({ name: 'home' });
    });
    return { Goto, step, remaingTime, CarPickupWizard };
  },
};
</script>
