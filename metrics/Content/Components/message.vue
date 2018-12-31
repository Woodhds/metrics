<template>
    <div class="md:w-1/2 sm:w-full border shadow px-4 py-4" ref="messages">
        <a class="block absolute" target="_blank" :href="'https://vk.com/wall' + message.Owner_Id + '_' + message.Id">
            <svg class="w-4 h-4" viewBox="0 0 25 25">
                <use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href="/images/icons.svg#key"/>
            </svg>
        </a>
        <figure class="flex flex-col items-center">
            <img class="w-auto h-32"
                 :src="message.Attachments && message.Attachments.length > 0 && message.Attachments[0].Photo ? message.Attachments[0].Photo.Sizes[4].Url : ''"/>
            <figcaption class="text-sm" v-html="message.Text"></figcaption>
        </figure>
        <div class="flex flex-row mt-6">
            <a :class="message.Reposts.User_reposted ? 'fill-red' : ''" class="cursor-pointer"
               @click="repost(message.Owner_Id, message.Id)">
                <span>{{ message.Reposts.Count }}</span>
                <svg class="w-4 h-4" viewBox="0 0 32 32">
                    <use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href='/images/icons.svg#thumbup'/>
                </svg>
            </a>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Prop, Emit } from 'vue-property-decorator';
    import axios from 'axios';
    import {VkMessage} from "../models/VkMessage";
    @Component
    export default class VkMessageComponent extends Vue {
        @Prop({default: null}) message: VkMessage;

        @Emit('repost')
        repost(owner_id: number, id: number) {
            axios.post('/api/repost/repost', [{owner_id, id}]).then(() => {
                this.message.Reposts.User_reposted = true;
            });
        }
    }
</script>