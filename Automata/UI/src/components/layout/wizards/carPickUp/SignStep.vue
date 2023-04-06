<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-sign-input v-model="points">
        <template #title>
          {{ $t('wizards.carPickUp.sign.title') }}
        </template>
      </wizard-sign-input>
    </v-sheet>
    <wizard-footer
      :nextBtnDisabled="nextBtnDisabled"
      @next="wizard.Goto(CarPickupWizard.DrivingLicenseStep)"
    >
    </wizard-footer>
  </div>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';

export default {
  name: 'sign-step',
  inject: ['wizard'],
  data() {
    return {
      points: 0,
      CarPickupWizard,
    };
  },
  mounted() {},
  created() {},
  methods: {},
  computed: {
    nextBtnDisabled() {
      return this.points < 100;
    },
  },
  watch: {},
  components: {},
};
</script>
<style scoped></style>
