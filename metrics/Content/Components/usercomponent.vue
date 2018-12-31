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
            <Message v-for="message of messages" :message="message" :key="message.Id + message.Owner_Id"
                 class="md:w-1/2 sm:w-full border shadow px-4 py-4">
            </Message>
            <ul class="flex list-reset mt-6" v-if="totalPages.length > 1">
                <li class="px-4 py-2 cursor-pointer" @click="page = item"
                    :class="[ item === page ? 'bg-blue text-white': '' ]" v-for="item of totalPages">{{item}}
                </li>
            </ul>
        </div>
        <div v-if="isLoading" class="loading"></div>
    </div>
</template>

<script lang="ts">
    import {Component, Watch} from 'vue-property-decorator';
    import axios from 'axios';
    import Vue from 'vue';
    import {VkUser} from "../models/user";
    import Dropdown from "./dropdown.vue";
    import {DataSourceResponse, VkMessage} from "../models/VkMessage";
    import Message from './message.vue';

    @Component({
        components: {Dropdown, Message}
    })
    export default class UserComponent extends Vue {
        users: VkUser[] = [];
        search: string = '';
        selected: VkUser | null = null;
        messages: VkMessage[] = [];
        isLoading: boolean = false;
        pageSize: number = 100;
        page: number = 1;
        total: number = 0;

        @Watch('page')
        pageChange() {
            this.searchMessages();
            window.scroll({top: 0, behavior: "smooth"});
        }

        beforeMount(): void {
            axios.get<VkUser[]>('/user/users').then(response => {
                this.users = response.data;
            });
        }

        searchMessages(): void {
            this.isLoading = true;
            axios.get<DataSourceResponse<VkMessage>>(`api/repost/user?search=${this.search}
                    &userId=${this.selected ? this.selected.UserId : null}
                    &page=${this.page}&pageSize=${this.pageSize}`)
                .then(response => {
                    this.messages = response.data.Data;
                    this.total = response.data.Total;
                }).then(() => (this.isLoading = false));
        }

        handleSelect(user: VkUser): void {
            this.selected = user;
        }

        get totalPages(): number[] {
            let arr = [];
            for (let i = 1; i <= Math.ceil(this.total / this.pageSize); i++) {
                arr.push(i)
            }

            return arr;
        }
    }
</script>