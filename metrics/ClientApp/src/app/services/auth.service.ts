import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {LoginModel} from "../login/loginmodel";
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private httpClient: HttpClient) {
  }

  auth(token: string): Observable<LoginModel> {
    return this.httpClient.post<LoginModel>('/api/account/login', { 'token': token });
  }


  static isLogged(): boolean {
    const token = localStorage.getItem('metrics-token');
    return token != null && token != '';
  }
}
