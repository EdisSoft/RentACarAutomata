<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content align-center justify-center d-flex text-h1">
      <div>
        {{ $t('wizards.carPickUp.final.title') }}
      </div>
    </v-sheet>
    <wizard-footer @next="Next()" :homeConfirm="false"> </wizard-footer>
  </div>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';
import router from '@/router';

export default {
  name: 'final-step',
  inject: ['wizard'],
  data() {
    return {
      CarPickupWizard,
    };
  },
  mounted() {},
  created() {},
  methods: {
    Next() {
      router.push({ name: 'home' });
    },
  },
  computed: {},
  watch: {},
  components: {},
};
</script>
<style scoped></style>
