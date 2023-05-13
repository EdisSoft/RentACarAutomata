<template>
  <v-card
    class="select-card"
    flat
    outlined
    @click="RekeszNyitas"
    :disabled="isRekeszNyitasLoading"
  >
    <v-card-title class="justify-center align-center">
      <v-btn text icon :loading="isRekeszNyitasLoading" class="text-h6">
        {{ cabinetNumber }}
      </v-btn>
      <v-icon v-if="isOpened" color="success"> mdi-lock-open-variant</v-icon>
      <v-icon v-else>mdi-lock</v-icon>
    </v-card-title>
  </v-card>
</template>

<script>
import { LockService } from '@/services/LockService';
import { useApi } from '@/utils/useApi';

export default {
  name: 'key-cabinet-item',
  data() {
    return {};
  },
  setup(props, context) {
    let [isRekeszNyitasLoading, RekeszNyitas] = useApi(async () => {
      let result = await LockService.OpenLock(props.cabinetNumber);
      context.emit('opened');
      return result;
    });
    return { isRekeszNyitasLoading, RekeszNyitas };
  },
  created() {},
  methods: {},
  computed: {},
  watch: {},
  props: {
    cabinetNumber: { type: Number },
    isOpened: { type: Boolean },
  },
};
</script>
