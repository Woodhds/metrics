import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http'
import { Injectable } from '@angular/core';
import { IAuthService } from '../services/abstract/IAuth';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private auth: IAuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler) {

    let request = req.clone();
    if (this.auth.currentUser && this.auth.currentUser.token) {
      request.headers.set('Authorization', `bearer ${this.auth.currentUser.token}`)
    }

    return next.handle(request);
  }
}