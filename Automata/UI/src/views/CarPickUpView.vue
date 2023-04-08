<template>
  <v-stepper v-model="step" class="wizard dense">
    <v-stepper-header>
      <v-stepper-step
        :complete="step > CarPickupWizard.BookingNumberOrQr"
        :step="CarPickupWizard.BookingNumberOrQr"
      >
        <span v-html="$t('wizards.carPickUp.bookingNumberOrQr.stepName')">
        </span>
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarPickupWizard.ChooseReservation"
        :step="CarPickupWizard.ChooseReservation"
      >
        <span v-html="$t('wizards.carPickUp.chooseReservation.stepName')">
        </span>
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarPickupWizard.EmailAdressStep"
        :step="CarPickupWizard.EmailAdressStep"
      >
        <span v-html="$t('wizards.carPickUp.emailAdress.stepName')"> </span>
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarPickupWizard.SignStep"
        :step="CarPickupWizard.SignStep"
      >
        <span v-html="$t('wizards.carPickUp.sign.stepName')"> </span>
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarPickupWizard.DrivingLicenseStep"
        :step="CarPickupWizard.DrivingLicenseStep"
      >
        <span v-html="$t('wizards.carPickUp.driverLicense.stepName')"> </span>
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarPickupWizard.IdCardStep"
        :step="CarPickupWizard.IdCardStep"
      >
        <span v-html="$t('wizards.carPickUp.idCardOrPassport.stepName')">
        </span>
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarPickupWizard.CreditCardStep"
        :step="CarPickupWizard.CreditCardStep"
      >
        <span v-html="$t('wizards.carPickUp.creditCard.stepName')"> </span>
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarPickupWizard.PayDepositStep"
        :step="CarPickupWizard.PayDepositStep"
      >
        <span v-html="$t('wizards.carPickUp.payDeposit.stepName')"> </span>
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarPickupWizard.PayRentalFeeStep"
        :step="CarPickupWizard.PayRentalFeeStep"
      >
        <span v-html="$t('wizards.carPickUp.payRentalFee.stepName')"> </span>
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step >= CarPickupWizard.FinalStep"
        :step="CarPickupWizard.FinalStep"
      >
        <span v-html="$t('wizards.carPickUp.final.stepName')"> </span>
      </v-stepper-step>
    </v-stepper-header>
    <v-stepper-items>
      <wizard-step-content :step="CarPickupWizard.BookingNumberOrQr">
        <booking-number-or-qr-step></booking-number-or-qr-step>
      </wizard-step-content>
      <wizard-step-content :step="CarPickupWizard.ChooseReservation">
        <choose-reservation-step></choose-reservation-step>
      </wizard-step-content>
      <wizard-step-content :step="CarPickupWizard.EmailAdressStep">
        <email-adress-step></email-adress-step>
      </wizard-step-content>
      <wizard-step-content :step="CarPickupWizard.SignStep">
        <sign-step></sign-step>
      </wizard-step-content>
      <wizard-step-content :step="CarPickupWizard.DrivingLicenseStep">
        <driving-license-step></driving-license-step>
      </wizard-step-content>
      <wizard-step-content :step="CarPickupWizard.IdCardStep">
        <id-card-step></id-card-step>
      </wizard-step-content>
      <wizard-step-content :step="CarPickupWizard.CreditCardStep">
        <credit-card-step></credit-card-step>
      </wizard-step-content>
      <wizard-step-content :step="CarPickupWizard.PayDepositStep">
        <pay-deposit-step></pay-deposit-step>
      </wizard-step-content>
      <wizard-step-content :step="CarPickupWizard.PayRentalFeeStep">
        <pay-rental-fee-step></pay-rental-fee-step>
      </wizard-step-content>
      <wizard-step-content :step="CarPickupWizard.FinalStep">
        <final-step></final-step>
      </wizard-step-content>
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
      wizardForm: computed(() => {
        return this.form;
      }),
      wizardStep: computed(() => {
        return this.step;
      }),
    };
  },
  setup(props, ctx) {
    const { Goto, step, form, SetFormValue } = useWizard(
      CarPickupWizard.BookingNumberOrQr
    );
    const { remaingTime } = useCountdown(5 * 60 * 1000, () => {
      router.push({ name: 'home' });
    });
    return { Goto, step, remaingTime, CarPickupWizard, form, SetFormValue };
  },
};
</script>
