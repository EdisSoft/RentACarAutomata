<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-document-input :preview="prevDocument">
        <template #title>
          {{ $t('wizards.carPickUp.driverLicense.title') }}
        </template>
      </wizard-document-input>
    </v-sheet>
    <wizard-footer
      @next="Next()"
      @back="Back()"
      :nextBtnText="nextBtnText"
      :back-btn="[front, back].some((s) => s)"
    >
    </wizard-footer>
  </div>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';

export default {
  name: 'driving-license-step',
  inject: ['wizard'],
  data() {
    return {
      front: null,
      back: null,
      prevDocument: null,
      CarPickupWizard,
    };
  },
  mounted() {},
  created() {},
  methods: {
    Next() {
      if (!this.front) {
        this.front =
          'https://developers.elementor.com/docs/assets/img/elementor-placeholder-image.png';
        this.prevDocument = this.front;
        return;
      }
      if (!this.back) {
        this.back =
          'https://s3-alpha.figma.com/hub/file/948140848/1f4d8ea7-e9d9-48b7-b70c-819482fb10fb-cover.png';
        this.prevDocument = this.back;
        return;
      }
      this.wizard.Goto(CarPickupWizard.CreditCardStep);
    },
    Back() {
      if (this.back) {
        this.back = null;
        this.prevDocument = this.front;
        return;
      }
      if (this.front) {
        this.front = null;
        this.prevDocument = null;
        return;
      }
    },
  },
  computed: {
    nextBtnText() {
      if (!this.front) {
        return this.$t('common.scanFront');
      }
      if (!this.back) {
        return this.$t('common.scanBack');
      }
      return null;
      return null;
    },
  },
  watch: {},
  components: {},
};
</script>
<style scoped></style>
