<template>
  <div class="fullscreen-layout">
    <v-container class="header">
      <v-row>
        <v-col cols="12">
          <div class="text-h3 text-center primary--text font-weight-medium">
            {{ $t('keyCabinetsTitle') }}
          </div>
        </v-col>
      </v-row>
    </v-container>
    <v-container class="body" fluid>
      <v-row>
        <v-col cols="12" md="6" class="d-flex flex-column gap-1rem">
          <key-cabinet-item v-for="n in 4" :key="n" :cabinet-number="n">
          </key-cabinet-item>
        </v-col>
        <v-col
          cols="12"
          offset-md1
          md="6"
          class="d-flex flex-column gap-1rem justify-end"
        >
          <key-cabinet-item v-for="n in 4" :key="n" :cabinet-number="n + 4">
          </key-cabinet-item>
        </v-col>
      </v-row>
    </v-container>
    <wizard-footer :homeConfirm="false" :nextBtn="false" class="footer">
    </wizard-footer>
  </div>
</template>

<script>
import router from '@/router';
import { computed } from 'vue';
import { useCountdown } from '@/utils/useCountdown';
import { AutomataService } from '@/services/AutomataService';
import { useApi } from '@/utils/useApi';
export default {
  name: 'Home',
  provide() {
    return {
      remaingTime: computed(() => {
        return this.remaingTime;
      }),
    };
  },
  setup() {
    let { remaingTime } = useCountdown(60 * 1000, () => {
      router.push({ name: 'welcome' });
    });

    return { remaingTime };
  },
  mounted() {},

  methods: {},
  computed: {},
};
</script>
<style lang="scss" scoped></style>
