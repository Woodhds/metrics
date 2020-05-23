import { Injectable } from '@angular/core';
import { User } from 'src/app/models/User';
import {HttpClient, HttpParams} from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<User>;
  public currentUserObs: Observable<User>;
  public get currentUser(): User {
    return this.currentUserSubject.value;
  }

  constructor(private httpClient: HttpClient) {
    this.currentUserSubject = new BehaviorSubject<User>(
      JSON.parse(localStorage.getItem('vkUser'))
    );

    this.currentUserObs = this.currentUserSubject.asObservable();
  }

  login(loginProvider: string): void {
    const httpParams = new HttpParams()
      .set('loginProvider', loginProvider)
      .set('returnUrl', location.href)

    window.location.href = `${environment.apiUrl}/auth/authorize/externallogin?` + httpParams.toString()
  }

  getUserInfo(token) {
    return this.httpClient
      .get<User>(`${environment.apiUrl}/auth/user`, {
        headers: {
          Authorization: 'Bearer ' + token
        }
      })
      .pipe(
        map(user => {
          user.Token = token;
          localStorage.setItem('vkUser', JSON.stringify(user));
          this.currentUserSubject.next(user);
          return user;
        })
      );
  }

  setToken(token: string) {
    return this.httpClient.post(`${environment.apiUrl}/auth/user/token?token=` + token, null)
  }

  logout(): void {
    localStorage.removeItem('vkUser');
    this.currentUserSubject.next(null);
  }
}
