import { Component, OnInit } from "@angular/core";
import { User } from "src/app/models/User";
import { IAuthService } from "src/app/services/abstract/IAuth";
import {AppConfigService} from "../../services/concrete/AppConfig/AppConfigService";

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.scss"]
})
export class LoginComponent implements OnInit {
  constructor(private authService: IAuthService, private appConfigService: AppConfigService) {}

  public user: User;

  ngOnInit() {
    this.user = this.authService.currentUser;
  }

  public get isAuthenticated(): Boolean {
    return this.user != null;
  }
}
