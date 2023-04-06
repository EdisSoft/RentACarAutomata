<template>
  <v-container
    ref="bg"
    fluid
    class="welcome fill-height yellow"
    @click="GoHome"
  >
    <v-row dense class="justify-space-around fill-height align-content-end">
      <v-col cols="12">
        <v-img
          ref="logo"
          class="logo"
          :src="require('@/assets/logos/game_logo.png')"
          contain
        >
        </v-img>
      </v-col>
      <v-col cols="12">
        <div
          class="primary-text primary--text text-h4 font-weight-medium d-flex align-end justify-center w-100 py-5 font-weight-bold start-text"
        >
          <div ref="startText">
            {{ $t('touchStart') }}
          </div>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import { gsap, Linear } from 'gsap';
export default {
  name: 'WelcomeView',
  data() {
    return {};
  },
  async mounted() {
    let showTl = gsap.timeline();
    showTl.from(this.$refs.bg, {
      opacity: 0,
      duration: 0.5,
    });
    showTl.from(this.$refs.logo.$el, {
      opacity: 0,
      duration: 0.3,
      ease: Linear.easeIn,
    });
    showTl.from(
      this.$refs.startText,
      {
        opacity: 0,
        duration: 0.3,
        ease: Linear.easeIn,
      },
      '<'
    );

    let moveTl = gsap.timeline({
      repeat: -1,
      delay: 5,
      repeatDelay: 5,
    });
    moveTl.to(this.$refs.startText, {
      duration: 0.4,
      scale: 0.9,
      ease: Linear.easeIn,
    });
    moveTl.to(this.$refs.startText, {
      duration: 0.4,
      scale: 1.1,
      ease: Linear.easeOut,
    });
    moveTl.to(this.$refs.startText, {
      duration: 0.4,
      scale: 0.9,
      ease: Linear.easeIn,
    });
    moveTl.to(this.$refs.startText, {
      duration: 0.4,
      scale: 1,
      ease: Linear.easeInOut,
    });
  },
  created() {},
  methods: {
    GoHome() {
      this.$router.push({ name: 'home' });
    },
  },
  computed: {},
  watch: {},
  components: {},
};
</script>
<style scoped>
.logo {
  position: absolute;
  inset: 0;
}
.start-text {
  isolation: isolate;
}
</style>
