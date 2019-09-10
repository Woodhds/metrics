import { Component } from 'vue-property-decorator';
import Vue from 'vue';
import { SelectMessageModel } from '../models/SelectMessageModel';

@Component
export default class BaseMixin extends Vue {
  isLoading: boolean = false;

  showLoading() {
    this.isLoading = true;
  }

  hideLoading() {
    this.isLoading = false;
  }
}