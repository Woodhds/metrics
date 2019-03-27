<template>
  <div class="mb-4 relative">
    <label class="text-sm font-bold text-gray-800 font-bold">{{Label}}</label>
    <div @click="toggleOpen" :class="opened ? 'outline-none shadow-outline': ''"
         class="px-10 py-4 shadow appearance-none cursor-pointer rounded">
      <figure v-if="Selected" class="flex items-center">
        <img class="w-16 h-16 rounded" :src="Selected.Avatar" :alt="Selected.FullName"/>
        <figcaption class="ml-3">{{ Selected.FullName }}</figcaption>
      </figure>
    </div>
    <ul v-if="List && opened" class="dropdown-list absolute p-4 z-20 bg-white w-full border shadow">
      <li class="flex flex-row items-center p-2 cursor-pointer" @click="select(item)" v-for="item of List"
          :key="item.UserId">
        <img class="mr-4" :src="item.Avatar" :alt="item.FullName"/>
        <span>{{item.FullName}}</span>
      </li>
    </ul>
  </div>
</template>

<script lang="ts">
  import Vue from 'vue';
  import {Component, Emit, Prop} from "vue-property-decorator";
  import {VkUser} from "../models/user";

  @Component
  export default class DropDownComponent extends Vue {

    @Prop({default: []}) List!: VkUser[];
    @Prop({default: ''}) Label: string;

    @Emit('select')
    select(user: VkUser) {
      this.toggleOpen();
      this.Selected = user
    };

    Selected: VkUser | null = null;
    opened: boolean = false;

    toggleOpen(): void {
      this.opened = !this.opened;
    };
  }
</script>