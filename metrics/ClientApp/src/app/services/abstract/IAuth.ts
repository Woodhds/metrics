import { User } from "src/app/models/User";
import {Observable} from "rxjs";

export abstract class IAuthService {
  abstract currentUser: Observable<User>;
  abstract login(authToken: string): Observable<User>;
  abstract logout(): void;
  abstract refresh(): void;
}
