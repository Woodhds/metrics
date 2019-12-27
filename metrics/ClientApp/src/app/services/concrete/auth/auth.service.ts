import { Injectable } from "@angular/core";
import { IAuthService } from "../../abstract/IAuth";
import { User } from "src/app/models/User";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, Observable } from "rxjs";
import { environment } from "../../../../environments/environment";
import { map } from "rxjs/operators";

@Injectable({
  providedIn: "root"
})
export class AuthService implements IAuthService {
  private currentUserSubject: BehaviorSubject<User>;
  public currentUserValue: Observable<User>;

  public get currentUser(): User {
    return this.currentUserSubject.value;
  }

  constructor(private httpClient: HttpClient) {
    this.currentUserSubject = new BehaviorSubject<User>(
      JSON.parse(localStorage.getItem("vkUser"))
    );

    this.currentUserValue = this.currentUserSubject.asObservable();
  }

  login(authToken: string): void {
    this.httpClient
      .post<User>(`${environment.apiUrl}/account/login`, { authToken })
      .pipe(
        map(user => {
          localStorage.setItem("vkUser", JSON.stringify(user));
          this.currentUserSubject.next(user);
          return user;
        })
      );
  }

  refresh(): void {
    throw new Error("Method not implemented.");
  }

  logout(): void {
    throw new Error("Method not implemented.");
  }
}
