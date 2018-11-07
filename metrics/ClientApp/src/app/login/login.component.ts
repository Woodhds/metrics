import {Component, OnInit} from '@angular/core';
import {AuthService} from "../services/auth.service";
import {Constants} from "../core/constants";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  public token: string;
  public clientId: string = Constants.ClientId;
  public redirectUrl = '';

  constructor(private authService: AuthService, private activatedRoute: ActivatedRoute) {
    this.redirectUrl = activatedRoute.snapshot.queryParams['redirectUrl'];
  }

  ngOnInit() {
  }

  handleLogin(): void {
    this.authService.auth(this.token, this.redirectUrl);
  }
}
