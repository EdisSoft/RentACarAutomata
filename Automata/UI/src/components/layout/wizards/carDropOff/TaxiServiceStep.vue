<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-select-input
        v-model="selectedItem"
        :options="[
          { id: 1, title: $t('common.yes') },
          { id: 2, title: $t('common.no') },
        ]"
      >
        <template #title>
          {{ $t('wizards.carDropOff.taxiService.title') }}
        </template>
      </wizard-select-input>
    </v-sheet>
    <wizard-footer
      @next="wizard.Goto(CarDropOffWizard.InsertCarKey)"
      :nextBtnDisabled="nextBtnDisabled"
    >
    </wizard-footer>
  </div>
</template>

<script>
import { CarDropOffWizard } from '@/enums/CarDropOffWizard';

export default {
  name: 'taxi-service-step',
  inject: ['wizard'],
  data() {
    return {
      selectedItem: null,
      CarDropOffWizard,
    };
  },
  mounted() {},
  created() {},
  methods: {},
  computed: {
    nextBtnDisabled() {
      return !this.selectedItem;
    },
  },
  watch: {},
  components: {},
};
</script>
<style scoped></style>
