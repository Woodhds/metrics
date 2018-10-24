import { Component, OnInit } from '@angular/core';
import {AuthService} from "../services/auth.service";
import {Constants} from "../core/constants";
import {NgForm} from "@angular/forms";
import {Router} from "@angular/router";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  public token: string;
  public message: string;
  public clientId: string = Constants.ClientId;
  constructor(private authService: AuthService, private router: Router) {
  }

  ngOnInit() {
  }

  handleLogin(form:NgForm) : void {
    var result = this.authService.auth(form.value);
    if(result) {

    }
  }
}
