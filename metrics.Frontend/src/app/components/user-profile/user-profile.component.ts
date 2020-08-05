import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';
import {AuthService} from "../../services/concrete/auth/auth.service";
import {UserToken} from "../../models/UserToken";
import {environment} from "../../../environments/environment";

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {
  form: FormGroup;
  data: Array<UserToken>;
  columns = ['Name', 'Value', 'LoginProvider', 'Actions']
  externalUrl = environment.apiUrl + '/auth/authorize/externalToken';

  constructor(private fb: FormBuilder, private authService: AuthService) {
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      token: ''
    })
    this.getData()
  }

  submit() {
    this.authService.setToken(this.form.get('token').value).subscribe(() => {
      this.getData()
    })
  }

  getData() {
    this.authService.getUserTokens().subscribe(data => {
      this.data = data;
    })
  }

  removeToken(name: string, loginProvider: string) {
    this.authService.removeToken(name, loginProvider).subscribe(() => {
      this.getData()
    })
  }
}
