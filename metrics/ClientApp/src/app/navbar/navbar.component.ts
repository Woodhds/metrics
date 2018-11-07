import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { UserInfo } from './user-info.model';
import {Observable} from "rxjs";

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {

  public userInfo: Observable<UserInfo>;
  constructor(private authService: AuthService) {
    this.userInfo = this.authService;
   }

  ngOnInit() {
    this.authService.getInfo();
  }

}
