<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-text-input v-model="text">
        <template #title>
          {{ $t('wizards.carPickUp.emailAdress.title') }}
        </template>
      </wizard-text-input>
    </v-sheet>
    <wizard-footer
      :nextBtnDisabled="nextBtnDisabled"
      @next="wizard.Goto(CarPickupWizard.SignStep)"
    >
    </wizard-footer>
  </div>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';

export default {
  name: 'email-adress-step',
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
