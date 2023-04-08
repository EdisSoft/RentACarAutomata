<template>
  <div class="wizard-input">
    <div class="input-title">
      <slot name="title"></slot>
    </div>
    <div class="input-text text-field">
      <v-text-field
        :value="value"
        @input="$emit('input', $event)"
        single-line
        counter
        :maxlength="maxlength"
      ></v-text-field>
    </div>
    <keyboard
      :value="value"
      @input="$emit('input', $event)"
      :maxlength="maxlength"
    ></keyboard>
  </div>
</template>

<script>
export default {
  name: 'wizard-text-input',
  data() {
    return {};
  },
  mounted() {},
  created() {},
  methods: {},
  computed: {},
  watch: {},
  props: {
    value: {},
    maxlength: {
      type: Number,
      default: 50,
    },
  },
};
</script>
