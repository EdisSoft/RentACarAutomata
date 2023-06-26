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
        <v-col
          cols="12"
          md="4"
          offset-md="1"
          class="d-flex flex-column gap-1rem"
        >
          <key-cabinet-item
            v-for="n in cabinets.left"
            :key="n.Id"
            :cabinet-number="n.Id"
            :is-opened="n.IsOpened"
            @opened="GetLockStatuses"
          >
          </key-cabinet-item>
        </v-col>
        <v-col
          cols="12"
          md="4"
          offset-md="2"
          class="d-flex flex-column gap-1rem justify-end"
        >
          <key-cabinet-item
            v-for="n in cabinets.right"
            :key="n.Id"
            :cabinet-number="n.Id"
            :is-opened="n.IsOpened"
            @opened="GetLockStatuses"
          >
          </key-cabinet-item>
        </v-col>
      </v-row>
    </v-container>
    <wizard-footer :homeConfirm="false" :nextBtn="false" class="footer">
      <v-btn :loading="lockStatusesLoading" x-large @click="GetLockStatuses">
        {{ $t('buttons.refresh') }}
      </v-btn>
    </wizard-footer>
  </div>
</template>

<script>
import router from '@/router';
import { computed, ref } from 'vue';
import { useCountdown } from '@/utils/useCountdown';
import { AutomataService } from '@/services/AutomataService';
import { useApi } from '@/utils/useApi';
import { LockService } from '@/services/LockService';
import chunk from 'lodash/chunk';
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
    let lockStatuses = ref([]);
    let { remaingTime } = useCountdown(60 * 1000, () => {
      router.push({ name: 'welcome' });
    });
    let [lockStatusesLoading, GetLockStatuses] = useApi(
      async () => {
        let result = await LockService.LockStatuses();
        lockStatuses.value = result;
        return result;
      },
      { silentError: true }
    );
    GetLockStatuses();
    return { remaingTime, lockStatuses, GetLockStatuses, lockStatusesLoading };
  },
  mounted() {},
  methods: {},
  computed: {
    cabinets() {
      let cabinets = [];
      let lockStatuses = this.lockStatuses;
      for (let i = 0; i < 9; i++) {
        let id = i + 1;
        let status = lockStatuses.find((f) => f.RekeszId == id);
        let cabinet = {
          Id: id,
          Name: id,
          IsOpened: status?.IsOpen ?? false,
        };
        cabinets.push(cabinet);
      }
      let [left, right] = chunk(cabinets, 4);
      return { left, right };
    },
  },
};
</script>
<style lang="scss" scoped></style>
