import {HttpInterceptor, HttpRequest, HttpHandler} from '@angular/common/http'
import { Injectable } from '@angular/core';

@Injectable()
export class CorsInterceptor implements HttpInterceptor {

  constructor() { }

  intercept(req: HttpRequest<any>, next: HttpHandler) {

    req = req.clone({
        setHeaders: {
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Methods': '*',
            'Access-Control-Allow-Headers': '*'
        }
    })

    return next.handle(req);
  }
}
