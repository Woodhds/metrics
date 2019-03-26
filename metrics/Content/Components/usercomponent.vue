<template>
    <div>
        <SwitchComponent @switchChange="switchChange" onText="Сайт" offText="Юзер"
                         :value="switchFromUser"></SwitchComponent>
        <div v-if="!switchFromUser" class="px-5 py-5 flex flex-col relative flex-wrap">
            <div class="flex flex-row mb-5 flex-wrap justify-between">
                <form @submit.prevent="searchMessages" class="rounded px-5 py-5 shadow-md sm:w-full md:w-1/2 sm:mb-4">
                    <Dropdown Label="Пользователь" :List="users" @select="handleSelect"></Dropdown>
                    <div class="mb-4 flex flex-col">
                        <label class="text-sm font-bold text-gray-900 block" for="search">Поиск</label>
                        <input id="search" name="search"
                               class="shadow px-5 py-2 leading-tight appearance-none rounded focus:outline-none focus:shadow-outline"
                               v-model="search"/>
                    </div>
                    <button type="submit" class="appearance-none bg-blue-800 hover:bg-blue-900 text-white py-2 px-5"
                            :disabled="!selected">Поиск
                    </button>
                </form>
                <div class="sm:w-full rounded px-5 py-5 shadow-md md:w-1/3 flex flex-col"
                     v-if="selectedMess.length > 0">
                    <div class="mb-4 relative">
                        <label for="timeout" class="text-sm font-bold text-gray-800 block">Таймаут с сек.</label>
                        <select id="timeout"
                                class="block appearance-none w-full bg-white border border-gray-100 hover:border-gray-300 px-4 py-2 pr-8 rounded shadow leading-tight focus:outline-none focus:shadow-outline"
                                v-model="timeout">
                            <option v-for="second of seconds" :value="second">{{second}}</option>
                        </select>
                        <div class="pointer-events-none dropdown-list__arrow absolute pin-r flex items-center px-2 text-gray-900">
                            <svg class="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
                                <path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z"/>
                            </svg>
                        </div>
                    </div>
                    <button @click="repostAll" class="appearance-none bg-blue-800 hover:bg-blue-900 text-white py-2 px-5">
                        Репост всего
                    </button>
                </div>
            </div>
            <div v-if="messages.length > 0" class="flex flex-col">
                <div class="mb-4 flex flex-row items-center">
                    <label class="text-sm font-bold text-gray-800 ml-4">Фильтровать по:</label>
                    <a @click="handleSortDate" class="no-underline cursor-pointer ml-4" nohref>Дате</a>
                    <a @click="handleSortCount" class="no-underline cursor-pointer ml-4" nohref>Кол-ву репостов</a>
                </div>
                <div class="flex flex-row flex-wrap">
                    <Message v-for="message of messages" @select="onSelect" :message="message"
                             :key="message.Id + message.Owner_Id">
                    </Message>
                </div>
                <ul class="flex list-reset mt-6" v-if="totalPages.length > 1">
                    <li class="px-4 py-2 cursor-pointer" @click="page = item"
                        :class="[ item === page ? 'bg-blue-800 text-white': '' ]" v-for="item of totalPages">{{item}}
                    </li>
                </ul>
            </div>
            <div v-if="isLoading" class="loading"></div>
        </div>
        <div v-else>
            <div class="flex flex-row flex-wrap">
                <Message v-for="message of messages" @select="onSelect" :message="message"
                         :key="message.Id + message.Owner_Id">
                </Message>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import {Component, Watch} from 'vue-property-decorator';
    import axios from 'axios';
    import Vue from 'vue';
    import {VkUser} from "../models/user";
    import Dropdown from "./dropdown.vue";
    import {VkMessage, VkRepostModel} from "../models/VkMessage";
    import Message from "./message.vue";
    import {FilterType} from "../models/FilterType";
    import {searchMessages, repost, getFromSite} from '../services/MessageService';
    import {SelectMessageModel} from "../models/SelectMessageModel";
    import SwitchComponent from "./switch.vue";

    @Component({
        components: {Dropdown, Message, SwitchComponent}
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
        selectedMess: SelectMessageModel[] = [];
        switchFromUser: boolean = false;

        @Watch('page')
        pageChange() {
            this.searchMessages();
            window.scroll({top: 0, behavior: "smooth"});
        }
        
        @Watch('switchFromUser')
        fetchFromSite() {
            if(this.switchFromUser) {
                getFromSite().then(response => {
                    this.messages = response.data.Data;
                    this.total = response.data.Total;
                })
            }
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
        
        switchChange(value: boolean): void {
            this.switchFromUser = value;
        }

        handleSortDate(): void {
            this.filterType = FilterType.Date;
        }

        handleSortCount(): void {
            this.filterType = FilterType.RepostCount;
        }

        showLoading() {
            this.isLoading = true;
        }

        hideLoading() {
            this.isLoading = false;
        }

        searchMessages(): void {
            this.showLoading();
            searchMessages(this.search, this.selected != null ? this.selected.UserId : 0, this.page, this.pageSize)
                .then(response => {
                    this.messages = response.data.Data;
                    this.total = response.data.Total;
                }).then(() => (this.hideLoading()));
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
            for (let i = 30; i <= 60; i++) {
                arr.push(i)
            }
            return arr;
        }

        onSelect(model: SelectMessageModel) {
            if (!model.IsSelect) {
                let idx = this.selectedMess.findIndex(d => d.Owner_Id == model.Owner_Id && d.Id == model.Id);
                if (idx !== -1) {
                    this.selectedMess.splice(idx, 1);
                }
            } else {
                this.selectedMess.push(model)
            }
        }

        repostAll() {
            this.showLoading();
            let mess = this.messages.filter(d => d.IsSelected);
            const reposts = mess.map(e => new VkRepostModel(e.Owner_Id, e.Id));
            repost(reposts, this.timeout).then().then(d => {
                this.hideLoading();
                mess.forEach(e => e.IsSelected = false);
            });
        }
    }
</script>