import {Component, OnDestroy, OnInit} from '@angular/core';
import {User} from 'src/app/models/User';
import * as signalR from '@microsoft/signalr';
import {environment} from '../../../environments/environment';
import {AuthService} from '../../services/concrete/auth/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {
  count = 0;
  hub: signalR.HubConnection = null;

  constructor(private authService: AuthService) {
  }

  public user: User = null;

  ngOnInit() {
    this.authService.currentUserObs.subscribe(d => {
      this.user = d;

      if (this.user) {

        const self = this;
        this.hub = new signalR.HubConnectionBuilder()
          .withUrl(`${environment.baseUrl}/notifications`, {
            accessTokenFactory(): string | Promise<string> {
              return self.user.Token;
            }
          })
          .build();

        this.hub.on('Count', (args: number) => {
          self.count = args;
        });

        this.hub.start().catch(reason => {
          console.log('Error', reason);
        });
      }
    });
  }

  public get isAuthenticated(): boolean {
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
