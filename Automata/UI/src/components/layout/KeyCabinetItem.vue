<template>
  <v-card
    class="select-card"
    flat
    outlined
    @click="RekeszNyitas"
    :disabled="isRekeszNyitasLoading"
  >
    <v-card-title class="justify-center">
      <v-btn text icon :loading="isRekeszNyitasLoading" class="text-h6">
        {{ cabinetNumber }}
      </v-btn>
    </v-card-title>
  </v-card>
</template>

<script>
import { AutomataService } from '@/services/AutomataService';
import { useApi } from '@/utils/useApi';

export default {
  name: 'key-cabinet-item',
  data() {
    return {};
  },
  setup(props) {
    let [isRekeszNyitasLoading, RekeszNyitas] = useApi(() => {
      return AutomataService.RekeszNyitas(props.cabinetNumber);
    });
    return { isRekeszNyitasLoading, RekeszNyitas };
  },
  created() {},
  methods: {},
  computed: {},
  watch: {},
  props: {
    cabinetNumber: { type: Number },
  },
};
</script>
