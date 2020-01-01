import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormControl, FormGroup} from "@angular/forms";
import {AppConfig} from "../../models/AppConfig";
import {AppConfigService} from "../../services/concrete/AppConfig/AppConfigService";

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit {

  form: FormGroup;
  public config: AppConfig;

  constructor(fb: FormBuilder, private appConfigService: AppConfigService) {
    this.form = fb.group({
      token: new FormControl('')
    })
    this.config = this.appConfigService.getConfig();
  }

  ngOnInit() {
  }

  get href() {
    return 'https://oauth.vk.com/authorize?client_id=' + this.config.ClientId +  `&display=page&redirect_uri=http://vk.com/blank.php&scope=111111111&response_type=token&v=5.85`
  }

  onSubmit(): void {

  }
}
