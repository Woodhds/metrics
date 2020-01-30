import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup} from "@angular/forms";
import {VkUserServiceService} from "../../services/vk-user-service.service";

@Component({
  selector: 'app-repost',
  templateUrl: './repost.component.html',
  styleUrls: ['./repost.component.scss']
})
export class RepostComponent implements OnInit {

  form: FormGroup;
  constructor(private formBuilder: FormBuilder, private userVkService: VkUserServiceService) {
    this.form = formBuilder.group({
      name: ''
    })
  }

  ngOnInit() {
  }

  onSubmit() {

  }

}
