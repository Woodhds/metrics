import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';
import {VkUserService} from '../../services/vk-user.service';
import {MatSnackBar} from "@angular/material/snack-bar";
import VkUserModel from "../../models/VkUserModel";
import {Observable} from "rxjs";
import {debounceTime} from "rxjs/operators";

@Component({
  selector: 'app-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.scss']
})
export class AddUserComponent implements OnInit {
  form: FormGroup;
  filteredOptions: Observable<VkUserModel[]>;

  constructor(private formBuilder: FormBuilder, private userService: VkUserService, private _snackbar: MatSnackBar) {
  }

  ngOnInit() {
    this.form = this.formBuilder.group({
      userId: ''
    });
    this.form.controls['userId'].valueChanges.pipe(debounceTime(1000))
      .subscribe((value: string) => {
        this.filteredOptions = this._filter(value)
      })
  }

  _filter(value: string) {
    return this.userService.search(value);
  }

  onSubmit() {
    this.userService.createUser((<VkUserModel>this.form.get('userId').value).Id)
    this._snackbar.open('Пользователь добавлен')
  }

  display(user: VkUserModel) {
    return user != null ? user.FullName : '';
  }
}
