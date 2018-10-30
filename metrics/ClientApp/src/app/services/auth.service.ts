import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {LoginModel} from "../login/loginmodel";
import { Observable } from 'rxjs';
import { UserInfo } from '../navbar/user-info.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private httpClient: HttpClient) {
  }

  static isLogged(): boolean {
    const token = localStorage.getItem('metrics-token');
    return token != null && token !== '';
  }

  auth(token: string): Observable<LoginModel> {
    return this.httpClient.post<LoginModel>('/api/account/login', { 'token': token });
  }

  getInfo(): Observable<UserInfo> {
    if (AuthService.isLogged()) {
      return this.httpClient.get<UserInfo>(`/api/account/info`);
    }
  }
}
