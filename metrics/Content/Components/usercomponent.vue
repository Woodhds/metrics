<template>
    <div class="bg-dark px-5 py-5 flex flex-col relative flex-wrap">
        <div class="flex flex-row mb-5 flex-wrap justify-between items-center">
            <form @submit.prevent="searchMessages" class="sm:w-full md:w-1/3 sm:mb-4">
                <Dropdown Label="Пользователь" :List="users" @select="handleSelect"></Dropdown>
                <div class="mb-4 flex flex-col">
                    <label class="text-sm font-bold text-grey block" for="search">Поиск</label>
                    <input id="search" name="search" class="shadow px-5 py-2 leading-tight appearance-none rounded focus:outline-none focus:shadow-outline"
                           v-model="search"/>
                </div>
                <button type="submit" class="appearance-none bg-blue hover:bg-blue-dark text-white py-2 px-5"
                        :disabled="!selected">Поиск
                </button>
            </form>
            <div class="sm:w-full md:w-1/3 flex flex-col" v-if="allRepostMessages.length > 0">
                <div class="mb-4 relative">
                    <label for="timeout" class="text-sm font-bold text-grey block">Таймаут с сек.</label>
                    <select id="timeout" class="block appearance-none w-full bg-white border border-grey-light hover:border-grey px-4 py-2 pr-8 rounded shadow leading-tight focus:outline-none focus:shadow-outline" v-model="timeout">
                        <option v-for="second of seconds" :value="second">{{second}}</option>
                    </select>
                    <div class="pointer-events-none absolute pin-y pin-r flex items-center px-2 text-grey-darker">
                        <svg class="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"><path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z"/></svg>
                    </div>
                </div>
                <button @click="repostAll" class="appearance-none bg-blue hover:bg-blue-dark text-white py-2 px-5">Репост всего</button>
            </div>
        </div>
        <div v-if="messages.length > 0" class="flex flex-col">
            <div class="mb-4 flex flex-row items-center">
                <label class="text-sm font-bold text-grey ml-4">Фильтровать по:</label>
                <a @click="handleSortDate" class="no-underline cursor-pointer ml-4" nohref>Дате</a>
                <a @click="handleSortCount" class="no-underline cursor-pointer ml-4" nohref>Кол-ву репостов</a>
            </div>
            <div class="flex flex-row flex-wrap">
                <Message v-for="message of messages" @select="onSelect" :message="message" :key="message.Id + message.Owner_Id"
                         class="md:w-1/2 sm:w-full border shadow px-4 py-4">
                </Message>
            </div>
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
    import {VkMessage, VkRepostModel} from "../models/VkMessage";
    import Message from './message.vue';
    import {FilterType} from "../models/FilterType";
    import {SelectMessageModel} from "../models/SelectMessageModel";
    import {searchMessages, repost} from '../services/MessageService';

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
        filterType: FilterType = FilterType.None;
        timeout: number = 0;
        allRepostMessages: VkRepostModel[] = [];

        @Watch('page')
        pageChange() {
            this.searchMessages();
            window.scroll({top: 0, behavior: "smooth"});
        }

        @Watch('filterType')
        filterChange() {
            switch (this.filterType) {
                case FilterType.Date:
                case FilterType.None:
                    this.messages.sort((a, b) => b.Date - a.Date);
                    break;
                case FilterType.RepostCount:
                    this.messages.sort((a, b) => a.Reposts.Count - b.Reposts.Count);
                    break;
            }
        }

        beforeMount(): void {
            axios.get<VkUser[]>('/user/users').then(response => {
                this.users = response.data;
            });
        }

        handleSortDate(): void {
            this.filterType = FilterType.Date;
        }

        handleSortCount(): void {
            this.filterType = FilterType.RepostCount;
        }

        searchMessages(): void {
            this.isLoading = true;
            searchMessages(this.search, this.selected != null ? this.selected.UserId : 0, this.page, this.pageSize)
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
        
        get seconds(): number[] {
            let arr = [];
            for(let i = 30; i <= 60; i++) {
                arr.push(i)
            }
            return arr;
        }
        
        onSelect(repostMessage: SelectMessageModel) {
            if(repostMessage.IsSelect) {
                this.allRepostMessages.push(new VkRepostModel(repostMessage.Owner_Id, repostMessage.Id))
            } else {
                this.allRepostMessages = this.allRepostMessages.filter(r => r.Id !== repostMessage.Id && r.Owner_Id !== repostMessage.Owner_Id)
            }
        }
        
        repostAll() {
            this.isLoading = true;
            repost(this.allRepostMessages, this.timeout).then().then(d => {
                this.isLoading = false;
                this.allRepostMessages = [];
            });
        }
    }
</script>