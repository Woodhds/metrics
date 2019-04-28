<template>
  <div class="vk-message border shadow px-4 py-4 relative mr-2 mb-2" ref="messages">
    <a
      class="block absolute"
      target="_blank"
      :href="'https://vk.com/wall' + message.Owner_Id + '_' + message.Id"
    >
      <svg class="w-4 h-4" viewBox="0 0 25 25">
        <use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href="/images/icons.svg#key"></use>
      </svg>
    </a>
    <div class="absolute block right-0">
      <SwitchComponent @switchChange="switchChange" :value="message.IsSelect"></SwitchComponent>
    </div>
    <ImageL :src="Images" :figcaption="message.Text"></ImageL>
    <div class="flex flex-row mt-6">
      <a
        :class="message.Reposts.User_reposted ? 'text-red-600' : ''"
        class="cursor-pointer items-center flex mr-5"
        @click="repost(message.Owner_Id, message.Id)"
      >
        <span>{{ message.Reposts.Count }}</span>
        <svg class="w-4 h-4 ml-2 fill-current hover:text-red-600" viewBox="0 0 32 32">
          <use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href="/images/icons.svg#thumbup"></use>
        </svg>
      </a>
      <a
        :class="message.Likes.User_Likes ? 'text-red-600' : ''"
        class="cursor-pointer items-center flex"
        @click="like"
      >
        <span>{{message.Likes.Count}}</span>
        <svg class="w-4 h-4 ml-2 fill-current hover:text-red-600" viewBox="0 0 32 32">
          <use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href="/images/icons.svg#heart"></use>
        </svg>
      </a>
    </div>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { Component, Prop } from "vue-property-decorator";
import { VkMessage, VkRepostModel } from "../models/VkMessage";
import SwitchComponent from "./switch.vue";
import ImageL from "./image.vue";
import { repost, like } from "../services/MessageService";
import { SelectMessageModel } from "../models/SelectMessageModel";

@Component({
  components: { SwitchComponent, ImageL }
})
export default class VkMessageComponent extends Vue {
  @Prop({ default: null }) message: VkMessage;
  currentImage: number = 0;

  repost(owner_id: number, id: number) {
    const vue = this;
    vue.$emit("showLoading");
    const model = new VkRepostModel(owner_id, id);
    repost([model])
      .then(() => {
        this.message.Reposts.User_reposted = true;
      })
      .then(() => {
        vue.$emit("hideLoading");
      });
  }

  get Images(): string[] {
    if (this.message.Attachments) {
      return this.message.Attachments.filter(
        img => img.Photo && img.Photo.Sizes[4]
      ).map(item => item.Photo.Sizes[4].Url);
    }

    return [];
  }

  switchChange(value: boolean) {
    this.message.IsSelected = value;
    this.$emit("select", <SelectMessageModel>{
      IsSelect: value,
      Id: this.message.Id,
      Owner_Id: this.message.Owner_Id
    });
  }

  like() {
    const vue = this;
    vue.$emit("showLoading");
    const model: VkRepostModel = {
      Id: this.message.Id,
      Owner_Id: this.message.Owner_Id
    };

    like(model)
      .then(() => {
        this.message.Likes.User_Likes = true;
      })
      .then(() => {
        vue.$emit("hideLoading");
      });
  }
}
</script>

<style lang="scss">
.image-point {
  top: 1px;
  left: 1px;
  transition: transform 0.5s ease;
}
</style>
