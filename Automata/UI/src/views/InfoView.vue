<template>
  <div class="scrollable-layout flex-column h-screen">
    <div class="scrollable-content">
      <iframe :src="manual" height="100%" width="100%"></iframe>
    </div>
    <v-sheet class="footer">
      <v-btn outlined x-large color="primary" :to="{ name: 'home' }">
        <v-icon>mdi-home</v-icon>
      </v-btn>
      <v-divider vertical class="mx-5"></v-divider>
      <div class="info-text">{{ remaingTime }}</div>
    </v-sheet>
  </div>
</template>

<script>
import manual from '@/assets/documents/Manual.pdf';
import router from '@/router';
import { useCountdown } from '@/utils/useCountdown';
export default {
  name: 'InfoView',
  data() {
    return {
      manual,
      width: window.innerWidth,
    };
  },
  setup() {
    const { remaingTime } = useCountdown(5 * 60 * 1000, () => {
      router.push({ name: 'home' });
    });

    return { remaingTime };
  },
  mounted() {},
  created() {},
  methods: {},
  computed: {},
  watch: {},
  components: {},
};
</script>
