<template>
  <ul class="flex mt-6 flex-wrap" v-if="totalPages.length > 1">
    <li
      class="px-4 py-2 cursor-pointer"
      @click="onChangePage(item)"
      :key="item"
      :class="[ item === page ? 'bg-blue-800 text-white': '' ]"
      v-for="item of totalPages"
    >{{item}}</li>
  </ul>
</template>

<script lang="ts">
import Vue from "vue";
import { Component, Prop, Emit } from "vue-property-decorator";

@Component
export default class PagerComponent extends Vue {
  @Prop() totalPages: number = 1;
  @Prop() page: number = 1;

  @Emit("changePage")
  onChangePage(t: number): number {
    return t;
  }
}
</script>