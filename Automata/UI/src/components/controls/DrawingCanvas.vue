<template>
  <div ref="wrapper" class="canvas-wrapper">
    <canvas
      :key="0"
      class="canvas"
      ref="canvas"
      :height="`${height}px`"
      :width="`${width}px`"
      @mousedown="OnMousedown"
      @mousemove="OnMousemove"
      @mouseup="OnMouseup"
      @touchstart="OnTouchstart"
      @touchmove="OnTouchmove"
    ></canvas>
    <v-fade-transition>
      <v-btn
        color="secondary"
        class="delete-icon"
        v-show="value > 5"
        x-large
        icon
        @click="Reset"
      >
        <v-icon>mdi-pen-remove</v-icon>
      </v-btn>
    </v-fade-transition>
  </div>
</template>

<script>
import { getCanvasAsFile, saveCanvas } from '@/utils/common';
export default {
  name: 'drawing-canvas',
  data() {
    return {
      key: 0,
      height: 400,
      width: 400,
      mouseDown: false,
      touchDown: false,
    };
  },
  mounted() {
    this.CreateCtx();
  },
  created() {},
  methods: {
    async CreateCtx() {
      this.canvas = this.$refs.canvas;
      this.height = this.$refs.wrapper.clientHeight - 10;
      this.width = this.$refs.wrapper.clientWidth;
      await this.$nextTick();
      this.ctx = this.canvas.getContext('2d');
      this.ctx.fillStyle = 'white';
      this.ctx.strokeStyle = 'black';
      this.ctx.lineJoin = 'round';
      this.ctx.lineWidth = 1;
      this.ctx.filter = 'blur(0.5px)';
      this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
    },
    RelPos(pt) {
      let rekt = pt.target.getBoundingClientRect();
      return [pt.pageX - rekt.left, pt.pageY - rekt.top];
    },
    DrawStart(pt) {
      this.ctx.beginPath();
      this.ctx.moveTo.apply(this.ctx, pt);
      this.ctx.stroke();
    },
    DrawMove(pt) {
      this.ctx.lineTo.apply(this.ctx, pt);
      this.ctx.stroke();
      this.$emit('input', this.value + 1);
    },
    PointerDown(e) {
      this.DrawStart(this.RelPos(e.touches ? e.touches[0] : e));
    },
    PointerMove(e) {
      this.DrawMove(this.RelPos(e.touches ? e.touches[0] : e));
    },

    OnMousedown(e) {
      this.PointerDown(e);
      this.mouseDown = true;
    },
    OnMousemove(e) {
      if (!this.mouseDown) {
        return;
      }
      this.PointerMove(e);
    },
    OnMouseup(e) {
      this.ctx.closePath(e);
      this.mouseDown = false;
    },
    OnTouchstart(e) {
      this.PointerDown(e);
      this.touchDown = true;
    },
    OnTouchmove(e) {
      if (!this.touchDown) {
        return;
      }
      this.PointerMove(e);
    },
    OnTouchend(e) {
      this.ctx.closePath(e);
      this.touchDown = false;
    },
    Save() {
      saveCanvas(this.canvas);
    },
    GetFile(filename) {
      return getCanvasAsFile(this.canvas, filename);
    },
    GetBase64() {
      return this.canvas.toDataURL();
    },
    Reset() {
      this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
      this.$emit('input', 0);
    },
  },
  computed: {},
  watch: {},
  props: {
    offsetTop: {
      type: Number,
      default: 0,
    },
    value: {
      type: Number,
      default: 0,
    },
    showClear: {
      type: Boolean,
      default: false,
    },
  },
};
</script>
<style scoped>
.canvas-wrapper {
  height: 100%;
  width: 100%;
  position: relative;
}
.canvas {
  border: 1px solid black;
}
.delete-icon {
  top: 0;
  right: 0;
  transform: translateX(calc(100% + 10px));
  position: absolute;
}
</style>
