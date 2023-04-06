<template>
  <div class="wizard-input">
    <div class="input-title">
      <slot name="title"></slot>
    </div>
    <div class="input-text d-flex justify-center gap-1rem">
      <v-btn
        v-for="option in optionsWithSelected"
        :key="option.id"
        color="primary"
        :outlined="!option.selected"
        x-large
        @click="Select(option)"
      >
        <v-icon large class="mr-5">
          {{
            option.selected ? 'mdi-check-circle-outline' : 'mdi-circle-outline'
          }}
        </v-icon>

        {{ option.text }}
      </v-btn>
    </div>
    <div></div>
  </div>
</template>

<script>
export default {
  name: 'wizard-select-input',
  data() {
    return {};
  },
  mounted() {},
  created() {},
  methods: {
    Select(e) {
      this.$emit('input', e);
    },
  },
  computed: {
    optionsWithSelected() {
      let selectedId = this.value?.id;
      return this.options.map((m) => {
        return {
          ...m,
          selected: m.id == selectedId,
        };
      });
    },
  },
  watch: {},
  props: {
    value: {},
    options: {
      type: Array,
      default() {
        return [];
      },
    },
  },
};
</script>
