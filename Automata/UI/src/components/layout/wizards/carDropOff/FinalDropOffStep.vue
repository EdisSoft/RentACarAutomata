<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-simple-input>
        <template #title>
          {{ $t('wizards.carDropOff.final.title') }}
        </template>
      </wizard-simple-input>
    </v-sheet>
    <wizard-footer
      @next="Next()"
      :homeConfirm="false"
      :nextBtnText="$t('buttons.finish')"
    >
    </wizard-footer>
  </div>
</template>

<script>
import { CarDropOffWizard } from '@/enums/CarDropOffWizard';
import router from '@/router';

export default {
  name: 'final-drop-off-step',
  inject: ['wizard'],
  data() {
    return {
      CarDropOffWizard,
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
