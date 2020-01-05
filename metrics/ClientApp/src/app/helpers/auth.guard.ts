import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { IAuthService } from '../services/abstract/IAuth';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: IAuthService, private router: Router) {
  }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    this.authService.currentUser.subscribe(user => {
      if (user) {
        return true
      }

      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
      return true
    });

    return true
  }
}
