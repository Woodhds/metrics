import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { UserInfo } from './user-info.model';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {

  public userInfo: UserInfo;
  constructor(private authService: AuthService) {
    this.authService.getInfo().subscribe((info: UserInfo) => this.userInfo = info);
   }

  ngOnInit() {
  }

}
