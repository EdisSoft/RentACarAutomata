<template>
  <div class="input fill-height">
    <div class="fill-height pa-4">
      <v-text-field v-model="input" solo></v-text-field>

      <keyboard v-model="input"></keyboard>
    </div>
  </div>
</template>

<script>
import Keyboard from '@/components/controls/Keyboard.vue';

export default {
  name: 'InputView',
  data() {
    return {
      input: '',
    };
  },
  mounted() {
    console.log(this.$vuetify);
  },
  created() {},
  methods: {},
  computed: {},
  watch: {},
  components: {
    Keyboard,
  },
};
</script>
