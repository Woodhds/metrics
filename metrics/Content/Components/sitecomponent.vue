<template>
  <div>
    <div class="flex flex-col items-center">
      <div class="flex flex-wrap w-full justify-between">
        <FormGroup
          label="Поиск"
          class="sm:w-full md:w-1/3"
          @input="e => search = e"
          id="search"
          :value="search"
        ></FormGroup>
        <RepostComponent class="sm:w-full md:w-1/3 justify-center"></RepostComponent>
      </div>
      <div class="flex flex-wrap">
        <Message
          v-for="message of showMessage"
          @select="onSelect"
          :message="message"
          F
          :key="message.Id + '_' + message.Owner_Id"
        ></Message>
      </div>
      <form class="flex flex-wrap" @submit.prevent="getFromSite">
        <Button text="Получить"></Button>
      </form>
    </div>
    <PagerComponent :page="page" :total-pages="50" @changePage="onChangePage"></PagerComponent>
    <div v-show="isLoading" class="loading"></div>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { Component, Prop, Model } from "vue-property-decorator";
import FormGroup from "./form-group.vue";
import Button from "./button.vue";
import Message from "./message.vue";
import { getFromSite } from "../services/MessageService";
import { VkMessage } from "../models/VkMessage";
import PagerComponent from "./pager.vue";
import RepostComponent from "./repostcomponent.vue";

@Component({
  components: {
    Button,
    Message,
    PagerComponent,
    RepostComponent,
    FormGroup
  }
})
export default class SiteComponent extends Vue {
  messages: VkMessage[] = [];
  page: number = 0;
  isLoading: boolean = false;
  render: boolean = false;
  search: string = "";

  getFromSite() {
    this.isLoading = true;
    this.page++;
    getFromSite(this.page).then(response => {
      this.isLoading = false;
      this.$set(this, "messages", [...this.messages, ...response.data.Data]);
    });
  }

  get showMessage(): VkMessage[] {
    return this.search === ""
      ? this.messages
      : this.messages.filter(d => d.Text.includes(this.search));
  }

  onChangePage(page: number): void {
    this.page = page;
  }

  mounted(): void {
    this.getFromSite();
  }
}
</script>