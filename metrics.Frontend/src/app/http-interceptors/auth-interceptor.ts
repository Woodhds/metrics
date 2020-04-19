import {HttpInterceptor, HttpRequest, HttpHandler} from '@angular/common/http'
import { Injectable } from '@angular/core';
import { IAuthService } from '../services/abstract/IAuth';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private auth: IAuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler) {

    const user = this.auth.currentUser;
    if (user && user.Token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${user.Token}`
        }
      })
    }

    return next.handle(req);
  }
}
