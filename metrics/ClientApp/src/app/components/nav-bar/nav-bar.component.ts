import { Component, OnInit } from '@angular/core';
import {User} from "../../models/User";
import {IAuthService} from "../../services/abstract/IAuth";

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent implements OnInit {

  private user: User = null;
  constructor(private userService: IAuthService) {
   }

   public get isAuthenticated(): Boolean {
    return this.user != null;
   }

  ngOnInit() {
    this.userService.currentUserObs.subscribe(e => {
      this.user = e;
    })
  }
}
