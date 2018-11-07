import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {LoginModel} from "../login/loginmodel";
import {BehaviorSubject} from 'rxjs';
import {UserInfo} from '../navbar/user-info.model';
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class AuthService extends BehaviorSubject<UserInfo> {

  constructor(private httpClient: HttpClient, private router: Router) {
    super({IsLogged: false, Avatar: '', FullName: ''});
  }

  auth(token: string, redirect: string = ''): void {
    this.httpClient.post<LoginModel>('/api/account/login', {'token': token}).subscribe(token => {
      if (token && token.accessToken != '') {
        localStorage.setItem('metrics-token', token.accessToken);
        this.getInfo(redirect);
      }
    });
  }

  static isLogged() {
    const token = localStorage.getItem('metrics-token');
    return token != null && token !== '';
  }

  getInfo(redirect: string = ''): void {
    if (AuthService.isLogged()) {
      this.httpClient.get<UserInfo>(`/api/account/info`).subscribe(
        e => {
          super.next({IsLogged: true, Avatar: e.Avatar, FullName: e.FullName});
          if (redirect && redirect != '') {
            this.router.navigateByUrl(redirect)
          }
        });
    }
  }
}
