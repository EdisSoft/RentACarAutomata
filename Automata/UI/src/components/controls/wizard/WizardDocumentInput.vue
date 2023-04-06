<template>
  <div class="wizard-input">
    <div class="input-title">
      <slot name="title"></slot>
    </div>
    <div class="input-text"></div>
    <div class="input-preview">
      <v-img v-if="preview" max-width="50vw" :src="preview" contain></v-img>
    </div>
  </div>
</template>

<script>
export default {
  name: 'wizard-document-input',
  data() {
    return {};
  },
  mounted() {},
  created() {},
  methods: {},
  computed: {},
  watch: {},
  props: {
    preview: {
      type: String,
      default: '',
    },
  },
};
</script>
