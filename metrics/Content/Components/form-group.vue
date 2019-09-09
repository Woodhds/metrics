<template>
  <div class="mb-4 flex flex-col">
    <label class="text-sm font-bold text-gray-900 block" :for="id">{{ label }}</label>
    <input
      :id="id"
      :type="type"
      :name="id"
      @input="onInput($event.target.value)"
      class="shadow px-5 py-2 leading-tight appearance-none rounded focus:outline-none focus:shadow-outline"
      :value="value"
    />
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { Prop, Component, Model } from "vue-property-decorator";

@Component
export default class FormGroup extends Vue {
  @Prop({ default: 'text' }) type: String;
  @Prop({ required: true }) id: String;
  @Model() value: String;
  @Prop({ default: '' }) label: String;

  onInput(value: String): void {
    this.$emit("input", value.trim());
  }
}
</script>