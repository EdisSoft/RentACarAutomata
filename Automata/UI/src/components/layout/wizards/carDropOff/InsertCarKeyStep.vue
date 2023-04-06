<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-simple-input>
        <template #title>
          {{ $t('wizards.carDropOff.insertCarKey.title') }}
        </template>
      </wizard-simple-input>
    </v-sheet>
    <wizard-footer @next="wizard.Goto(CarDropOffWizard.Final)"> </wizard-footer>
  </div>
</template>

<script>
import { CarDropOffWizard } from '@/enums/CarDropOffWizard';

export default {
  name: 'insert-car-key-step',
  inject: ['wizard'],
  data() {
    return {
      CarDropOffWizard,
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
