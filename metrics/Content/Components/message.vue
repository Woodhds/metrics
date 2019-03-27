<template>
  <div class="vk-message border shadow px-4 py-4 relative mr-2 mb-2" ref="messages">
    <a class="block absolute" target="_blank" :href="'https://vk.com/wall' + message.Owner_Id + '_' + message.Id">
      <svg class="w-4 h-4" viewBox="0 0 25 25">
        <use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href="/images/icons.svg#key"/>
      </svg>
    </a>
    <div class="absolute block right-0">
      <SwitchComponent @switchChange="switchChange" :value="message.IsSelect"></SwitchComponent>
    </div>
    <figure class="flex flex-col items-center">
      <div class="flex items-center">
        <a @click="prev" v-if="totalImage > 1" nohref class="border border-gray-300 hover:shadow rounded-full block cursor-pointer p-4 mr-6">
          <svg class="fill-current h-4 w-4" xmlns:xlink="http://www.w3.org/1999/xlink">
            <use xlink:href="/images/icons.svg#arrow-left"></use>
          </svg>
        </a>
        <div class="flex flex-col mb-2">
          <img v-if="totalImage > 0" class="w-auto h-32 mb-2"
             :src="message.Attachments && message.Attachments.length > 0 && message.Attachments[currentImage].Photo ? message.Attachments[currentImage].Photo.Sizes[4].Url : ''"/>
          <ul class="flex flex-wrap items-center justify-center">
            <li @click="setCurrent(image)" v-for="image of imagePoints" :key="image" class="rounded-full relative border border-gray-300 h-4 w-4 p-1 ml-2 cursor-pointer">
              <span style="top: 1px;left: 1px;" v-if="image === currentImage" class="bg-gray-500 top-0 left-0 w-3 h-3 absolute rounded-full block"></span>
            </li>
          </ul>
        </div>
        <a @click="next" v-if="totalImage > 1" nohref class="border border-gray-300 hover:shadow rounded-full block cursor-pointer p-4 ml-6">
          <svg class="fill-current h-4 w-4" xmlns:xlink="http://www.w3.org/1999/xlink">
            <use xlink:href="/images/icons.svg#arrow-right"></use>
          </svg>
        </a>
      </div>
      <figcaption class="text-sm leading-normal word-break max-h-screen overflow-y-auto"
                  v-html="message.Text"></figcaption>
    </figure>
    <div class="flex flex-row mt-6">
      <a :class="message.Reposts.User_reposted ? 'text-red-600' : ''" class="cursor-pointer items-center flex mr-5"
         @click="repost(message.Owner_Id, message.Id)">
        <span>{{ message.Reposts.Count }}</span>
        <svg class="w-4 h-4 ml-2 fill-current" viewBox="0 0 32 32">
          <use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href='/images/icons.svg#thumbup'/>
        </svg>
      </a>
      <a :class="message.Likes.User_Likes ? 'text-red-600' : ''" class="cursor-pointer items-center flex"
         @click="like">
        <span>{{message.Likes.Count}}</span>
        <svg class="w-4 h-4 ml-2 fill-current" viewBox="0 0 32 32">
          <use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href='/images/icons.svg#heart'/>
        </svg>
      </a>
    </div>
  </div>
</template>

<script lang="ts">
  import Vue from 'vue';
  import {Component, Prop} from 'vue-property-decorator';
  import {VkMessage, VkRepostModel} from "../models/VkMessage";
  import SwitchComponent from './switch.vue';
  import {repost, like} from '../services/MessageService'
  import {SelectMessageModel} from "../models/SelectMessageModel";

  @Component({
    components: {SwitchComponent}
  })
  export default class VkMessageComponent extends Vue {
    @Prop({default: null}) message: VkMessage;
    currentImage: number = 0;

    repost(owner_id: number, id: number) {
      const vue = this;
      vue.$emit('showLoading');
      const model = new VkRepostModel(owner_id, id);
      repost([model]).then(() => {
        this.message.Reposts.User_reposted = true;
      }).then(() => {
        vue.$emit('hideLoading');
      });
    }
    
    get imagePoints(): number[] {
      return Array.from(Array(this.totalImage).keys());
    }

    get totalImage(): number {
      return this.message.Attachments.filter(value => value.Photo).length;
    }

    next() {
      if (this.currentImage < this.totalImage - 1) {
        this.currentImage++;
      }
    }

    prev() {
      if (this.currentImage > 0) {
        this.currentImage--;
      }
    }
    
    setCurrent(image: number) {
      this.currentImage = image;
    }

    switchChange(value: boolean) {
      this.message.IsSelected = value;
      this.$emit('select', <SelectMessageModel>{
        IsSelect: value,
        Id: this.message.Id,
        Owner_Id: this.message.Owner_Id
      })
    }

    like() {
      const vue = this;
      vue.$emit('showLoading');
      const model: VkRepostModel = {
        Id: this.message.Id,
        Owner_Id: this.message.Owner_Id
      };

      like(model).then(() => {
        this.message.Likes.User_Likes = true;
      }).then(() => {
        vue.$emit('hideLoading');
      })
    }
  }
</script>