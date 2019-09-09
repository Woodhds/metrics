<template>
  <button
    class="appearance-none bg-gray-800 hover:bg-gray-900 text-white py-2 px-5 rounded"
    @click="onClick"
    :disabled="disabled"
  >{{ text }}</button>
</template>

<script lang="ts">
import Vue from "vue";
import { Component, Prop, Emit } from "vue-property-decorator";

@Component
export default class Button extends Vue {
  @Prop({ default: false }) disabled: Boolean;
  @Prop() text: String;
  @Emit("click")
  onClick() {}
}
</script>