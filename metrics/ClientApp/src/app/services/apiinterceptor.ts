import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpHeaders} from "@angular/common/http";
import {Observable} from "rxjs";
import {Injectable} from "@angular/core";

@Injectable()
export class ApiInterceptor implements HttpInterceptor{
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = localStorage.getItem('metrics-token');
    const request = req.clone({ headers: req.headers.set('Authorization', 'bearer ' + token) });

    return next.handle(request);
  }
}
