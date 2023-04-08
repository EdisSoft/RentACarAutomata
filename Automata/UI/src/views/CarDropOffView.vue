<template>
  <v-stepper v-model="step" class="wizard">
    <v-stepper-header>
      <v-stepper-step
        :complete="step > CarDropOffWizard.PlateNumber"
        :step="CarDropOffWizard.PlateNumber"
      >
        <span v-html="$t('wizards.carDropOff.plateNumber.stepName')"> </span>
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarDropOffWizard.ParkingInformation"
        :step="CarDropOffWizard.ParkingInformation"
      >
        <span v-html="$t('wizards.carDropOff.parkingInformation.stepName')">
        </span>
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarDropOffWizard.TaxiService"
        :step="CarDropOffWizard.TaxiService"
      >
        <span v-html="$t('wizards.carDropOff.taxiService.stepName')"> </span>
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarDropOffWizard.InsertCarKey"
        :step="CarDropOffWizard.InsertCarKey"
      >
        <span v-html="$t('wizards.carDropOff.insertCarKey.stepName')"> </span>
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step >= CarDropOffWizard.Final"
        :step="CarDropOffWizard.Final"
      >
        <span v-html="$t('wizards.carDropOff.final.stepName')"> </span>
      </v-stepper-step>
    </v-stepper-header>
    <v-stepper-items>
      <wizard-step-content :step="CarDropOffWizard.PlateNumber">
        <plate-number-step></plate-number-step>
      </wizard-step-content>
      <wizard-step-content :step="CarDropOffWizard.ParkingInformation">
        <parking-information-step></parking-information-step>
      </wizard-step-content>
      <wizard-step-content :step="CarDropOffWizard.TaxiService">
        <taxi-service-step :key="step"></taxi-service-step>
      </wizard-step-content>
      <wizard-step-content :step="CarDropOffWizard.InsertCarKey">
        <insert-car-key-step></insert-car-key-step>
      </wizard-step-content>
      <wizard-step-content :step="CarDropOffWizard.Final">
        <final-drop-off-step></final-drop-off-step>
      </wizard-step-content>
    </v-stepper-items>
  </v-stepper>
</template>

<script>
import { useWizard } from '@/utils/useWizard';
import { computed } from 'vue';
import { CarDropOffWizard } from '@/enums/CarDropOffWizard';
import { useCountdown } from '@/utils/useCountdown';
import router from '@/router';

export default {
  name: 'CarDropOffView',
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
      CarDropOffWizard.PlateNumber
    );
    const { remaingTime } = useCountdown(5 * 60 * 1000, () => {
      router.push({ name: 'home' });
    });
    return { Goto, step, remaingTime, CarDropOffWizard, form, SetFormValue };
  },
};
</script>
