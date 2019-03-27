<template>
  <div>
    <div class="form-switch inline-block align-middle" @click="switchChange">
      <input v-model="isToggleOn" type="checkbox" class="form-switch-checkbox"/>
      <label class="form-switch-label"></label>
    </div>
    <label class="text-xs text-gray-dark">{{ `${ isToggleOn ? onText : offText }` }}</label>
  </div>
</template>
<script lang="ts">
  import Vue from 'vue';
  import {Component, Prop, Emit} from 'vue-property-decorator';

  @Component
  export default class SwitchComponent extends Vue {
    @Prop({type: String, default: ''}) offText: string = '';
    @Prop({type: String, default: ''}) onText: string = '';
    @Prop({type: Boolean, default: false}) value: boolean = false;

    isToggleOn: boolean = false;

    mounted() {
      this.isToggleOn = this.value;
    }

    @Emit('switchChange')
    switchChange() {
      this.isToggleOn = !this.isToggleOn;
      return this.isToggleOn;
    }
  }
</script>