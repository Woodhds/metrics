import { User } from "src/app/models/User";

export abstract class IAuthService {
  abstract currentUser: User;
  abstract login(authToken: string): void;
  abstract logout(): void;
  abstract refresh(): void;
}
