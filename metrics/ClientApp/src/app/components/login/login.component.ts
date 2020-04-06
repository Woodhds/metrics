import { Component, OnDestroy, OnInit } from "@angular/core";
import { User } from "src/app/models/User";
import { IAuthService } from "src/app/services/abstract/IAuth";
import * as signalR from "@microsoft/signalr";

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.scss"]
})
export class LoginComponent implements OnInit, OnDestroy {
  count: Number = 0;
  hub: signalR.HubConnection = null;

  constructor(private authService: IAuthService) {}

  public user: User = null;

  ngOnInit() {
    this.authService.currentUserObs.subscribe(d => {
      this.user = d;

      const self = this;
      this.hub = new signalR.HubConnectionBuilder()
        .withUrl("/notifications", {
          accessTokenFactory(): string | Promise<string> {
            return self.user.Token;
          }
        })
        .build();

      this.hub.on("Count", (args: Number) => {
        console.log("Receive", args);
        self.count = args;
      });

      this.hub.start().catch(reason => {
        console.log("Error", reason);
      });
    });
  }

  public get isAuthenticated(): Boolean {
    return this.user != null;
  }

  logout(): void {
    this.authService.logout();
  }

  ngOnDestroy(): void {
    if (this.hub) {
      this.hub.stop().catch(f => {
        console.log(f);
      });
    }
  }
}
