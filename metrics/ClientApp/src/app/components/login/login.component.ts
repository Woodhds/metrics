import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/User';
import { IAuthService }from 'src/app/services/abstract/IAuth';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  constructor(private authService: IAuthService) { }

  public user: User;
  ngOnInit() {
    this.user = this.authService.currentUser;
  }

}
