import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import {VkUserService} from '../../services/vk-user.service';
import {MatSnackBar} from "@angular/material/snack-bar";

@Component({
  selector: 'app-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.scss']
})
export class AddUserComponent implements OnInit {
  form: FormGroup;
  constructor(private formBuilder: FormBuilder, private userService: VkUserService, private _snackbar: MatSnackBar) {}

  ngOnInit() {
    this.form = this.formBuilder.group({
      userId: ''
    });
  }

  onSubmit() {
    this.userService.createUser(this.form.get('userId').value)
    this._snackbar.open('Пользователь добавлен')
  }
}
