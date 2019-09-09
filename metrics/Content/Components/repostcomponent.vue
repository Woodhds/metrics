<template>
  <div class="flex flex-col">
    <div><strong>Выбрано {{ selectedMess.length }} сообщений</strong></div>
    <div class="rounded px-5 py-5 shadow-md flex flex-col">
      <div class="mb-4 relative">
        <label for="timeout" class="text-sm font-bold text-gray-800 block">Таймаут с сек.</label>
        <select
          id="timeout"
          class="block appearance-none w-full bg-white border border-gray-100 hover:border-gray-300 px-4 py-2 pr-8 rounded shadow leading-tight focus:outline-none focus:shadow-outline"
          v-model="timeout"
        >
          <option v-for="second of seconds" :value="second" :key="second">{{second}}</option>
        </select>
        <div
          class="pointer-events-none dropdown-list__arrow absolute right-0 flex items-center px-2 text-gray-900"
        >
          <svg class="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
            <path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z" />
          </svg>
        </div>
      </div>
      <Button @click="repostAll" :disabled="selectedMess.length === 0" text="Репост всего"></Button>
    </div>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { Component, Mixins, Prop } from "vue-property-decorator";
import BaseMixin from "../mixins/base";
import { SelectMessageModel } from "../models/SelectMessageModel";
import { VkRepostModel } from "../models/VkMessage";
import { repost } from "../services/MessageService";
import Button from './button.vue';

@Component({
  components: {
    Button
  }
})
export default class RepostComponent extends Mixins(BaseMixin) {
  @Prop() timeout: number = 40;

  get seconds(): number[] {
    let arr = [];
    for (let i = 30; i <= 60; i++) {
      arr.push(i);
    }
    return arr;
  }

  repostAll() {
    this.showLoading();
    const reposts = this.selectedMess.map(
      e => new VkRepostModel(e.Owner_Id, e.Id)
    );
    repost(reposts, this.timeout)
      .then()
      .then(d => {
        this.hideLoading();
        this.$set(this, "selectedMess", new Array<SelectMessageModel>());
      });
  }
}
</script>