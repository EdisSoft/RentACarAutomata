<template>
  <div class="wizard-input select">
    <div class="input-title">
      <slot name="title"></slot>
    </div>
    <div class="input-text d-flex flex-wrap gap-1rem">
      <v-card
        v-for="option in optionsWithSelected"
        class="select-card"
        :class="{ selected: option.selected, primary: option.selected }"
        flat
        :dark="option.selected"
        :key="option.id"
        max-width="300"
        min-width="300"
        outlined
        @click="Select(option)"
      >
        <v-card-title>
          <v-icon large class="mr-4">
            {{
              option.selected
                ? 'mdi-check-circle-outline'
                : 'mdi-circle-outline'
            }}
          </v-icon>
          {{ option.title }}
        </v-card-title>
        <v-card-text v-if="option.text">
          {{ option.text }}
        </v-card-text>
      </v-card>
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
