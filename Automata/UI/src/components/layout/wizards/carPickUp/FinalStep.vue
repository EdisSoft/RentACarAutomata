<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content align-center justify-center d-flex text-h1">
      <div ref="thanks">
        {{ $t('wizards.carPickUp.final.title') }}
      </div>
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
import { CarPickupWizard } from '@/enums/CarPickupWizard';
import router from '@/router';
import { gsap } from 'gsap';

export default {
  name: 'final-step',
  inject: ['wizard'],
  data() {
    return {
      CarPickupWizard,
    };
  },
  mounted() {
    gsap.from(this.$refs.thanks, {
      delay: 0.3,
      duration: 0.3,
      opacity: 0,
      ease: 'power1.inOut',
    });
  },
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
