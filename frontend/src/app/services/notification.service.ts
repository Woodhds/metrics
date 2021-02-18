import {Injectable} from "@angular/core";
import {BehaviorSubject} from "rxjs";
import * as signalR from '@microsoft/signalr';
import {HubConnection} from "@microsoft/signalr";
import {environment} from "../../environments/environment";
import {AuthService} from "./auth.service";

@Injectable({
  providedIn: "root"
})
export default class NotificationService {
  private _hub: BehaviorSubject<signalR.HubConnection> = new BehaviorSubject<HubConnection>(null);

  get hub() {
    return this._hub.asObservable();
  }

  constructor(private authService: AuthService) {
  }

  connect(): void {


    this.authService.currentUserObs.subscribe(user => {

      if (user) {

        const hub = new signalR.HubConnectionBuilder()
          .withUrl(`${environment.baseUrl}/notifications`, {
            accessTokenFactory(): string | Promise<string> {
              return user.Token;
            }
          })
          .build();

        this._hub.next(hub)
        this.startHub();
      }
    });
  }

  private startHub() : void {
    if (this._hub.value) {
      this._hub.value.onclose(this.tryReconnect)
      this._hub.value.start().catch(this.tryReconnect);
    }
  }

  private tryReconnect(error?: Error) {
    if (error) {
      console.log('Error', error);
      setTimeout(this.startHub.bind(this), 60000)
    }
  }

}
