<template>
  <div class="mb-4 flex flex-col">
    <label class="text-sm font-bold text-gray-900 block" :for="id">{{ label }}</label>
    <input
      :id="id"
      :type="type"
      :name="id"
      @input="e => $emit('input', e.target.value)"
      class="shadow px-5 py-2 leading-tight appearance-none rounded focus:outline-none focus:shadow-outline"
      v-model="model"
    />
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { Prop, Component, Model } from "vue-property-decorator";

@Component
export default class FormGroup extends Vue {
  @Prop() type: String = "text";
  @Prop({ required: true }) id: String = "";
  @Model() model: String;
  @Prop() label: String = '';
}
</script>