import { Component, OnInit } from "@angular/core";
import { User } from "src/app/models/User";
import { IAuthService } from "src/app/services/abstract/IAuth";
import * as signalR from "@microsoft/signalr";

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.scss"]
})
export class LoginComponent implements OnInit {
  count: Number = 0;

  constructor(private authService: IAuthService) {}

  public user: User = null;

  ngOnInit() {
    this.authService.currentUserObs.subscribe(d => {
      this.user = d;
    });

    if (this.isAuthenticated) {
      const self = this;
      const hub = new signalR.HubConnectionBuilder()
        .withUrl("/notifications", {
          accessTokenFactory(): string | Promise<string> {
            return self.user.Token;
          }
        })
        .build();

      hub.on("Сount", (args: Number) => {
        console.log(args);
        self.count = args;
      });

      hub.start().catch(reason => {
        console.log(reason);
      });
    }
  }

  public get isAuthenticated(): Boolean {
    return this.user != null;
  }

  logout(): void {
    this.authService.logout();
  }
}
