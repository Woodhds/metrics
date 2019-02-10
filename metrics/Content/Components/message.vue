<template>
    <div class="vk-message border shadow px-4 py-4 relative mr-2 mb-2" ref="messages">
        <a class="block absolute" target="_blank" :href="'https://vk.com/wall' + message.Owner_Id + '_' + message.Id">
            <svg class="w-4 h-4" viewBox="0 0 25 25">
                <use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href="/images/icons.svg#key"/>
            </svg>
        </a>
        <div class="absolute block pin-r">
            <SwitchComponent @switchChange="switchChange" :value="message.IsSelect"></SwitchComponent>
        </div>
        <figure class="flex flex-col items-center">
            <img class="w-auto h-32 mb-3"
                 :src="message.Attachments && message.Attachments.length > 0 && message.Attachments[0].Photo ? message.Attachments[0].Photo.Sizes[4].Url : ''"/>
            <figcaption class="text-sm leading-normal word-break max-h-screen overflow-y-auto" v-html="message.Text"></figcaption>
        </figure>
        <div class="flex flex-row mt-6">
            <a :class="message.Reposts.User_reposted ? 'fill-red' : ''" class="cursor-pointer mr-4"
               @click="repost(message.Owner_Id, message.Id)">
                <span>{{ message.Reposts.Count }}</span>
                <svg class="w-4 h-4" viewBox="0 0 32 32">
                    <use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href='/images/icons.svg#thumbup'/>
                </svg>
            </a>
            <a :class="message.Likes.User_Likes ? 'fill-red' : ''" class="cursor-pointer"
            @click="like">
                <span>{{message.Likes.Count}}</span>
                <svg class="w-4 h-4" viewBox="0 0 32 32">
                    <use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href='/images/icons.svg#heart'/>
                </svg>
            </a>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Prop } from 'vue-property-decorator';
    import {VkMessage, VkRepostModel} from "../models/VkMessage";
    import SwitchComponent from './switch.vue';
    import {repost, like} from '../services/MessageService'
    import {SelectMessageModel} from "../models/SelectMessageModel";
    
    @Component({
        components: {SwitchComponent}
    })
    export default class VkMessageComponent extends Vue {
        @Prop({default: null}) message: VkMessage;
        
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