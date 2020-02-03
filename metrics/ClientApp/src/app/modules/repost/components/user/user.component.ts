import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup} from "@angular/forms";
import {interval, Observable} from "rxjs";
import VkUserModel from "../../models/VkUserModel";
import {VkUserService} from "../../services/vk-user.service";
import {debounceTime, distinctUntilChanged, map} from "rxjs/operators";
import {VkMessage} from "../../models/VkMessageModel";

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {
  form: FormGroup;
  users: Observable<VkUserModel[]>;
  messages: Observable<VkMessage[]>;
  constructor(private userService: VkUserService, private fb: FormBuilder) { }

  ngOnInit() {
    this.form =  this.fb.group({
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
    return value.toLowerCase().replace(/\s/g, '');
  }
}
