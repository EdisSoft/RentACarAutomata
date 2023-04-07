<template>
  <div class="wizard-input" v-intersect="OnIntersect">
    <div class="input-title">
      <slot name="title"></slot>
    </div>
    <div class="input-text"></div>

    <div class="d-flex justify-center align-center">
      <div class="canvas-container">
        <drawing-canvas
          ref="input"
          :key="key"
          :value="value"
          @input="$emit('input', $event)"
          :offsetTop="91"
        ></drawing-canvas>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'wizard-sign-input',
  data() {
    return {
      key: 1,
    };
  },
  mounted() {},
  created() {},
  methods: {
    OnIntersect() {
      this.key++;
    },
    GetBase64Image() {
      return this.$refs.input.GetBase64();
    },
  },

  watch: {},
  props: {
    value: {},
  },
};
</script>
<style scoped>
.canvas-container {
  height: min(35vh, 50vw);
  width: calc(min(35vh, 50vw) * 1.7778);
}
</style>
