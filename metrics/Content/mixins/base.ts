import { Component } from 'vue-property-decorator';
import Vue from 'vue';
import { SelectMessageModel } from '../models/SelectMessageModel';

@Component
export default class BaseMixin extends Vue {
  selectedMess: SelectMessageModel[] = [];
  isLoading: boolean = false;

  showLoading() {
    this.isLoading = true;
  }

  hideLoading() {
    this.isLoading = false;
  }


  onSelect(model: SelectMessageModel) {
    let idx = this.selectedMess.findIndex(
      d => d.Owner_Id == model.Owner_Id && d.Id == model.Id
    );
    if (!model.IsSelect && idx !== -1) {
      this.$set(
        this,
        "selectedMess",
        this.selectedMess.filter((_, i) => i !== idx)
      );
    } else {
      this.$set(this, "selectedMess", [...this.selectedMess, model]);
    }
  }
}