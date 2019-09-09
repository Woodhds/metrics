<template>
  <div>
    <div class="flex flex-col">
      <div class="flex flex-wrap">
        <Message
          v-for="message of messages"
          @select="onSelect"
          :message="message"
          :key="message.Id + '_' + message.Owner_Id"
        ></Message>
      </div>
      <form @submit.prevent="getFromSite">
        <Button text="Получить"></Button>
      </form>
    </div>
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

@Component({
  components: {
    Button
  }
})
export default class SiteComponent extends Vue {
  messages: VkMessage[] = [];
  page: number = 0;
  isLoading: boolean = false;

  getFromSite() {
    this.isLoading = true;
    this.page++;
    getFromSite(this.page).then(response => {
      this.isLoading = false;
      this.$set(this, "messages", [...this.messages, ...response.data.Data]);
    });
  }
}
</script>