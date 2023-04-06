<template>
  <div :class="keyboardClass"></div>
</template>

<script>
import { AppFunctions } from '@/functions/AppFunctions';
import Keyboard from 'simple-keyboard';
import 'simple-keyboard/build/css/index.css';
let ct = 1;
export default {
  name: 'keyboard',
  data: () => ({
    keyboard: null,
    keyboardClass: 'simple-keyboard-' + ct++,
  }),
  mounted() {
    let language = AppFunctions.GetLanguage(this.$i18n.locale);
    this.keyboard = new Keyboard(this.keyboardClass, {
      onChange: this.OnChange,
      onKeyPress: this.OnKeyPress,
      maxLength: this.maxlength,
      layout: {
        ...language.keyboard,
      },
      display: {
        '{lock}': 'caps',
        '{spec}': '!#1',
        '{space}': ' ',
        '{backspace}': this.$t('keyboard.backspace'),
      },
    });
  },
  methods: {
    OnChange(input) {
      this.$emit('input', input);
    },
    OnKeyPress(button) {
      this.$emit('oKeyPress', button);
      if (button === '{shift}' || button === '{lock}') {
        this.HandleShift();
      }
      if (button === '{spec}') {
        this.HandleSpec();
      }
    },
    HandleShift() {
      let currentLayout = this.keyboard.options.layoutName;
      let shiftToggle = currentLayout === 'default' ? 'shift' : 'default';
      this.keyboard.setOptions({
        layoutName: shiftToggle,
      });
    },
    HandleSpec() {
      let currentLayout = this.keyboard.options.layoutName;
      let layout = currentLayout != 'spec' ? 'spec' : 'default';
      console.log(currentLayout, layout);
      this.keyboard.setOptions({
        layoutName: layout,
      });
    },
  },
  watch: {
    value(input) {
      this.keyboard.setInput(input);
    },
  },
  props: {
    value: {
      type: String,
    },
    maxlength: {
      type: Number,
    },
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped></style>
