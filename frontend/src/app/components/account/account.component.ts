import {Component, OnInit} from '@angular/core';
import {AuthService} from '../../services/auth.service';
import {ActivatedRoute, Router} from '@angular/router';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit {

  constructor(private authService: AuthService, private activatedRoute: ActivatedRoute, private router: Router) {
  }

  ngOnInit() {

    this.authService.currentUserObs.subscribe(user => {
      if (user) {
        this.router.navigate(['/']);
      }
    });

    this.activatedRoute.queryParamMap.subscribe(params => {

      const token = params.get('access_token');
      if (token) {
        this.authService.getUserInfo(token).subscribe();
      }
    });
  }

  authorize(loginProvider) {
    this.authService.login(loginProvider);
  }
}
