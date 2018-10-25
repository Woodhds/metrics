import { Component, OnInit } from '@angular/core';
import {AuthService} from "../services/auth.service";
import {Constants} from "../core/constants";
import {FormGroup, NgForm} from "@angular/forms";
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

  handleLogin(): void {
    this.authService.auth(this.token).subscribe(data => {
      if (data && data.accessToken != '') {
        localStorage.setItem('metrics-token', data.accessToken)
        this.router.navigate(['/user'])
      } else {
        this.message = 'Ошибка авторизации';
      }
    });
  }
}
