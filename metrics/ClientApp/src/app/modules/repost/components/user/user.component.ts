import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup} from "@angular/forms";
import {Observable} from "rxjs";
import VkUserModel from "../../models/VkUserModel";
import {VkUserService} from "../../services/vk-user.service";
import {debounceTime, distinctUntilChanged, finalize, map} from "rxjs/operators";
import {VkMessage} from "../../models/VkMessageModel";
import {VkMessageService} from "../../services/vk-message.service";

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {
  form: FormGroup;
  users: Observable<VkUserModel[]>;
  messages: VkMessage[];
  loading: boolean = false;

  constructor(private userService: VkUserService, private fb: FormBuilder, private vkMessageService: VkMessageService) {
  }

  ngOnInit() {
    this.form = this.fb.group({
      user: '',
      search: ''
    });
    this.form.get('user').valueChanges.pipe(debounceTime(1000), distinctUntilChanged()).subscribe(value => this._filter(value));
  }

  private _filter(value: string): void {
    const filterValue = this._normalizeValue(value);
    this.users = this.userService.getUsers(filterValue);
  }

  private _normalizeValue(value: string): string {
    if (!value) return value;
    return value.toLowerCase().replace(/\s/g, '');
  }

  onSubmit() {
    this.loading = true;
    this.vkMessageService.getFromUser(
      this.form.get('user').value, 1, 100,
      this.form.get('search').value)
      .pipe(finalize(() => this.loading = false))
      .subscribe(data => {
        this.messages = data.Data;
      })
  }

  fromUnixTime(value: number): string {
    return new Date(value * 1000).toLocaleString();
  }
}
