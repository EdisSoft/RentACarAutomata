<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-simple-input>
        <template #title>
          {{ $t('wizards.carPickUp.payment.title') }}
        </template>
      </wizard-simple-input>
    </v-sheet>
    <wizard-footer @next="wizard.Goto(CarPickupWizard.FinalStep)">
    </wizard-footer>
  </div>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';

export default {
  name: 'payment-step',
  inject: ['wizard'],
  data() {
    return {
      CarPickupWizard,
    };
  },
  mounted() {},
  created() {},
  methods: {},
  computed: {},
  watch: {},
  components: {},
};
</script>
<style scoped></style>
