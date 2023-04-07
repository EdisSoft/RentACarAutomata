<template>
  <v-dialog
    eager
    @input="change"
    value="true"
    :max-width="width"
    :persistent="persistent"
    scrollable
    @keydown.esc="choose(false)"
  >
    <v-card tile>
      <v-toolbar
        v-if="Boolean(title)"
        :color="color"
        dense
        flat
        class="pt-2 mb-1"
      >
        <v-icon v-if="Boolean(icon)" left>{{ icon }}</v-icon>
        <v-toolbar-title v-text="title" />
      </v-toolbar>
      <v-card-text class="body-1 text-body-2 py-3" v-html="message" />
      <v-card-actions>
        <v-spacer />
        <v-btn
          v-if="buttonFalse"
          x-large
          :color="buttonFalseColor"
          text
          @click="choose(false)"
        >
          {{ buttonFalseText || $t('common.no') }}
        </v-btn>
        <v-btn
          v-if="buttonTrue"
          x-large
          :color="buttonTrueColor"
          :loading="loading"
          @click="choose(true)"
        >
          {{ buttonTrueText || $t('common.yes') }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import { NotificationFunctions } from '@/functions/NotificationFunctions';
export default {
  name: 'confirm-modal',
  data() {
    return {
      value: false,
      loading: false,
    };
  },
  mounted() {
    document.addEventListener('keyup', this.onEnterPressed);
  },
  destroyed() {
    document.removeEventListener('keyup', this.onEnterPressed);
  },
  methods: {
    onEnterPressed(e) {
      if (e.keyCode === 13) {
        e.stopPropagation();
        this.choose(true);
      }
    },
    async choose(value) {
      if (value) {
        this.loading = true;
        try {
          await this.successAction();
        } catch (error) {
          NotificationFunctions.AjaxError({ error });
          this.loading = false;
          return;
        }
      }
      this.$emit('result', value);
      this.value = value;
      this.$destroy();
    },
    change(res) {
      this.$destroy();
    },
  },
  props: {
    buttonTrueColor: {
      type: String,
      default: 'primary',
    },
    buttonTrue: {
      type: Boolean,
      default: true,
    },
    buttonTrueText: {
      type: String,
      default: '',
    },
    buttonFalseColor: {
      type: String,
      default: 'secondary',
    },
    buttonFalse: {
      type: Boolean,
      default: true,
    },
    buttonFalseText: {
      type: String,
      default: '',
    },
    color: {
      type: String,
      default: '',
    },
    icon: {
      type: String,
      default: 'fa-exclamation-triangle',
    },
    message: {
      type: String,
      required: true,
    },
    persistent: Boolean,
    title: {
      type: String,
    },
    width: {
      type: Number,
      default: 450,
    },
    successAction: {
      type: Function,
      default: () => {},
    },
  },
};
</script>
