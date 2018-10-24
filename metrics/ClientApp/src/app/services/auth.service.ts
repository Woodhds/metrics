import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private httpClient: HttpClient) {
  }

  auth(token: string): boolean {
    let authtoken = "";
    this.httpClient.post('/account/login', {token: token}).subscribe((data: string) => authtoken = data);
    if(token && token != '') {
      localStorage.setItem('metrics-token', authtoken);
      return true;
    }
    return false;
  }

  static isLogged(): boolean {
    const token = localStorage.getItem('metrics-token');
    return token != null && token != '';
  }
}
