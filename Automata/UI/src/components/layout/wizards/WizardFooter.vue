<template>
  <v-sheet class="wizard-footer">
    <v-btn
      :disabled="loading"
      outlined
      x-large
      color="primary"
      @click="GoToHome"
    >
      <v-icon>mdi-home</v-icon>
    </v-btn>
    <v-divider vertical></v-divider>
    <div class="info-text">
      {{ remaingTime }}
    </div>
    <v-spacer></v-spacer>
    <slot></slot>
    <v-btn
      v-if="backBtn"
      :disabled="loading"
      color="secondary"
      class="mr-2"
      x-large
      @click="$emit('back')"
    >
      {{ $t('buttons.back') }}
    </v-btn>
    <v-btn
      v-if="nextBtn"
      color="primary"
      :loading="loading"
      x-large
      :disabled="nextBtnDisabled"
      @click="$emit('next')"
    >
      {{ nextBtnText || $t('buttons.continue') }}
    </v-btn>
  </v-sheet>
</template>

<script>
import { NotificationFunctions } from '@/functions/NotificationFunctions';

export default {
  name: 'wizard-footer',
  inject: ['remaingTime'],
  data() {
    return {};
  },
  mounted() {},
  created() {},
  methods: {
    async GoToHome() {
      if (this.homeConfirm) {
        let success = await NotificationFunctions.ConfirmDialog(
          this.$t('confirms.goToHome.title'),
          this.$t('confirms.goToHome.question')
        );
        if (!success) {
          return;
        }
      }
      this.$router.push({ name: 'home' });
    },
  },
  computed: {},
  watch: {},
  props: {
    nextBtnText: {
      type: String,
      default: '',
    },
    nextBtnDisabled: {
      type: Boolean,
      default: false,
    },
    loading: {
      type: Boolean,
      default: false,
    },
    nextBtn: {
      type: Boolean,
      default: true,
    },
    backBtn: {
      type: Boolean,
      default: false,
    },
    homeConfirm: {
      type: Boolean,
      default: true,
    },
  },
};
</script>
