<template>
  <v-stepper v-model="step" class="wizard" eager>
    <v-stepper-header>
      <v-stepper-step
        :complete="step > CarDropOffWizard.PlateNumber"
        :step="CarDropOffWizard.PlateNumber"
      >
        {{ $t('wizards.carDropOff.plateNumber.stepName') }}
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarDropOffWizard.ParkingInformation"
        :step="CarDropOffWizard.ParkingInformation"
      >
        {{ $t('wizards.carDropOff.parkingInformation.stepName') }}
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarDropOffWizard.TaxiService"
        :step="CarDropOffWizard.TaxiService"
      >
        {{ $t('wizards.carDropOff.taxiService.stepName') }}
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarDropOffWizard.InsertCarKey"
        :step="CarDropOffWizard.InsertCarKey"
      >
        {{ $t('wizards.carDropOff.insertCarKey.stepName') }}
      </v-stepper-step>
      <v-divider></v-divider>
      <v-stepper-step
        :complete="step > CarDropOffWizard.Final"
        :step="CarDropOffWizard.Final"
      >
        {{ $t('wizards.carDropOff.final.stepName') }}
      </v-stepper-step>
    </v-stepper-header>
    <v-stepper-items>
      <v-stepper-content :step="CarDropOffWizard.PlateNumber">
        <plate-number-step></plate-number-step>
      </v-stepper-content>
      <v-stepper-content :step="CarDropOffWizard.ParkingInformation">
        <parking-information-step></parking-information-step>
      </v-stepper-content>
      <v-stepper-content :step="CarDropOffWizard.TaxiService">
        <taxi-service-step :key="step"></taxi-service-step>
      </v-stepper-content>
      <v-stepper-content :step="CarDropOffWizard.InsertCarKey">
        <insert-car-key-step></insert-car-key-step>
      </v-stepper-content>
      <v-stepper-content :step="CarDropOffWizard.Final">
        <final-drop-off-step></final-drop-off-step>
      </v-stepper-content>
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
    };
  },
  setup(props, ctx) {
    const { Goto, step } = useWizard(CarDropOffWizard.PlateNumber);
    const { remaingTime } = useCountdown(5 * 60 * 1000, () => {
      router.push({ name: 'home' });
    });
    return { Goto, step, remaingTime, CarDropOffWizard };
  },
};
</script>
