import {Component, OnInit} from '@angular/core';
import {User} from 'src/app/models/User';
import {AuthService} from '../../services/auth.service';
import NotificationService from '../../services/notification.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  count = 0;

  constructor(private authService: AuthService, private notificationService: NotificationService) {
  }

  public user: User = null;

  ngOnInit() {
    this.authService.currentUserObs.subscribe(d => {
      this.user = d;
    });

    this.notificationService.hub.subscribe(hub => {
      if (!hub) {return;}

      hub.on('Count', (args: number) => {
        this.count = args;
      });
    });

    this.notificationService.connect();
  }

  public get isAuthenticated(): boolean {
    return this.user != null;
  }

  logout(): void {
    this.authService.logout();
  }
}
