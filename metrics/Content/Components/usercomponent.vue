<template>
  <div class="px-5 py-5 flex flex-col relative flex-wrap">
    <div class="flex flex-row mb-5 flex-wrap justify-between">
      <form
        @submit.prevent="searchMessages"
        class="rounded px-5 py-5 shadow-md sm:w-full md:w-1/2 sm:mb-4"
      >
        <Dropdown label="Пользователь" :List="users" @select="handleSelect"></Dropdown>
        <FormGroup label="Поиск" @input="e => search = e" id="search" :value="search"></FormGroup>
        <Button :disabled="!selected" text="Поиск"></Button>
      </form>
      <RepostComponent @select="e => selectedMess = [...e]" class="sm:w-full md:w-1/3" :timeout="timeout"></RepostComponent>
    </div>
    <div v-if="messages.length > 0" class="flex flex-col">
      <div class="flex flex-row flex-wrap justify-center">
        <Message
          v-for="message of messages"
          @select="onSelect"
          :message="message"
          :is-select="selectedMess.some(d => d.Id === message.Id && d.Owner_Id === message.Owner_Id)"
          :key="message.Id + '_' + message.Owner_Id"
        ></Message>
      </div>
      <PagerComponent
        v-if="selectedMess.length > 0"
        :page="page"
        :total-pages="totalPages"
        @changePage="onChangePage"
      ></PagerComponent>
    </div>
    <div v-show="isLoading" class="loading"></div>
  </div>
</template>

<script lang="ts">
import { Component, Watch, Mixins } from "vue-property-decorator";
import axios from "axios";
import Vue from "vue";
import { VkUser } from "../models/user";
import Dropdown from "./dropdown.vue";
import { VkMessage, VkRepostModel } from "../models/VkMessage";
import Message from "./message.vue";
import { searchMessages, repost } from "../services/MessageService";
import { SelectMessageModel } from "../models/SelectMessageModel";
import SwitchComponent from "./switch.vue";
import PagerComponent from "./pager.vue";
import FormGroup from "./form-group.vue";
import Button from "./button.vue";
import BaseMixin from "../mixins/base";
import RepostComponent from "./repostcomponent.vue";

@Component({
  components: {
    Dropdown,
    Message,
    SwitchComponent,
    PagerComponent,
    FormGroup,
    Button,
    RepostComponent
  }
})
export default class UserComponent extends Mixins(BaseMixin) {
  users: VkUser[] = [];
  search: string = "";
  selected: VkUser | null = null;
  messages: VkMessage[] = [];
  isLoading: boolean = false;
  pageSize: number = 100;
  page: number = 1;
  total: number = 0;
  timeout: number = 40;
  switchFromUser: boolean = false;
  selectedMess: SelectMessageModel[] = [];

  @Watch("page")
  pageChange() {
    this.searchMessages();
    window.scroll({ top: 0, behavior: "smooth" });
  }

  beforeMount(): void {
    axios.get<VkUser[]>("/user/users").then(response => {
      this.users = response.data;
    });
  }

  searchMessages(): void {
    this.showLoading();
    searchMessages(
      this.search,
      this.selected != null ? this.selected.UserId : 0,
      this.page,
      this.pageSize
    )
      .then(response => {
        this.$set(this, "selectedMessa", new Array<SelectMessageModel>());
        this.messages = response.data.Data;
        this.total = response.data.Total;
      })
      .then(() => this.hideLoading());
  }

  handleSelect(user: VkUser): void {
    this.selected = user;
  }

  get totalPages(): number[] {
    let arr = [];
    for (let i = 1; i <= Math.ceil(this.total / this.pageSize); i++) {
      arr.push(i);
    }

    return arr;
  }

  onChangePage(page: number): void {
    this.page = page;
  }
}
</script>