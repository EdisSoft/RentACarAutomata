<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-text-input v-model="text" :maxlength="15">
        <template #title>
          {{ $t('wizards.carDropOff.plateNumber.title') }}
        </template>
      </wizard-text-input>
    </v-sheet>
    <wizard-footer
      :nextBtnDisabled="nextBtnDisabled"
      @next="wizard.Goto(CarDropOffWizard.ParkingInformation)"
    >
    </wizard-footer>
  </div>
</template>

<script>
import { CarDropOffWizard } from '@/enums/CarDropOffWizard';

export default {
  name: 'plate-number-step',
  inject: ['wizard'],
  data() {
    return {
      text: '',
      CarDropOffWizard,
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
