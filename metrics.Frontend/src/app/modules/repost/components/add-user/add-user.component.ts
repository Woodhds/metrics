import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import {VkUserService} from '../../services/vk-user.service';

@Component({
  selector: 'app-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.scss']
})
export class AddUserComponent implements OnInit {
  form: FormGroup;
  constructor(private formBuilder: FormBuilder, private userService: VkUserService) {}

  ngOnInit() {
    this.form = this.formBuilder.group({
      userId: ''
    });
  }

  onSubmit() {
    this.userService.createUser(this.form.get('userId').value)
  }
}
