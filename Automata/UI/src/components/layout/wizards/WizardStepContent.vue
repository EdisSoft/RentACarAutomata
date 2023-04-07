<template>
  <v-stepper-content :step="step">
    <slot v-if="wizardStep == step"></slot>
  </v-stepper-content>
</template>

<script>
export default {
  name: 'wizard-step-content',
  inject: ['wizardStep'],
  data() {
    return {};
  },
  mounted() {},
  created() {},
  methods: {},
  computed: {},
  watch: {},
  props: {
    step: {
      type: Number,
      required: true,
    },
  },
};
</script>
