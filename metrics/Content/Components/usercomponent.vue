<template>
    <div class="bg-dark px-5 py-5 flex flex-col relative">
        <form @submit.prevent="searchMessages" class="flex flex-col sm:w-full md:w-1/3 mb-5">
            <Dropdown Label="Пользователь" :List="users" @select="handleSelect"></Dropdown>
            <div class="mb-4 flex flex-col">
                <label class="text-sm font-bold text-grey block" for="search">Поиск</label>
                <input id="search" name="search" class="shadow px-5 py-2 leading-tight appearance-none"
                       v-model="search"/>
            </div>
            <button type="submit" class="appearance-none bg-blue hover:bg-blue-dark text-white py-2 px-5"
                    :disabled="!selected">Поиск
            </button>
        </form>
        <div v-if="messages.length > 0" class="flex flex-row flex-wrap">
            <div v-for="message of messages" :key="message.Id + message.Owner_Id" class="md:w-1/2 sm:w-full border shadow px-4 py-4">
                <figure class="flex flex-col items-center">
                    <img class="w-auto h-32"
                         :src="message.Attachments && message.Attachments.length > 0 && message.Attachments[0].Photo ? message.Attachments[0].Photo.Sizes[4].Url : ''"/>
                    <figcaption class="text-sm" v-html="message.Text"></figcaption>
                </figure>
                <div class="flex flex-row mt-6">
                    <a :class="message.Reposts.User_reposted ? 'fill-red' : ''"
                       @click="repost(message.Owner_Id, message.Id)">
                        <span>{{ message.Reposts.Count }}</span>
                        <svg class="w-4 h-4" viewBox="0 0 32 32">
                            <use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href='/images/icons.svg#thumbup'/>
                        </svg>
                    </a>
                </div>
            </div>
        </div>
        <div v-if="isLoading" class="loading"></div>
    </div>
</template>

<script lang="ts">
    import {Component} from 'vue-property-decorator';
    import axios from 'axios';
    import Vue from 'vue';
    import {VkUser} from "../models/user";
    import Dropdown from "./dropdown.vue";
    import {DataSourceResponse, VkMessage} from "../models/VkMessage";

    @Component({
        components: {Dropdown}
    })
    export default class UserComponent extends Vue {
        users: VkUser[] = [];
        search: string = '';
        selected: VkUser | null = null;
        messages: VkMessage[] = [];
        isLoading: boolean = false;

        beforeMount(): void {
            axios.get<VkUser[]>('/user/users').then(response => {
                this.users = response.data;
            });
        }

        searchMessages(): void {
            this.isLoading = true;
            axios.get<DataSourceResponse<VkMessage>>(`api/repost/user?search=${this.search}&userId=${this.selected ? this.selected.UserId : null}&page=1&pageSize=100`)
                .then(response => {
                    this.messages = response.data.Data;
                }).then(() => (this.isLoading = false));
        }

        handleSelect(user: VkUser): void {
            this.selected = user;
        }

        repost(owner_id: number, id: number) {
            this.isLoading = true;
            axios.post('/api/repost/repost', [{owner_id, id}]).then(() => {
                this.isLoading = false;
            });
        }
    }
</script>