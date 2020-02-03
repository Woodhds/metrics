import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "../../helpers/auth.guard";
import { MatInputModule } from "@angular/material/input";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatButtonModule } from "@angular/material/button";
import { RepostComponent } from "./components/repost/repost.component";
import { HTTP_INTERCEPTORS, HttpClientModule } from "@angular/common/http";
import { AuthInterceptor } from "../../http-interceptors/auth-interceptor";
import { ReactiveFormsModule } from "@angular/forms";
import { AddUserComponent } from "./components/add-user/add-user.component";
import { UserComponent } from './components/user/user.component';

const routes: Routes = [
  { path: "", canActivate: [AuthGuard], component: RepostComponent },
  { path: "add", canActivate: [AuthGuard], component: AddUserComponent },
  { path: "user", canActivate: [AuthGuard], component: UserComponent }
];

@NgModule({
  declarations: [RepostComponent, AddUserComponent, UserComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    MatInputModule,
    MatAutocompleteModule,
    ReactiveFormsModule,
    MatButtonModule,
    HttpClientModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, multi: true, useClass: AuthInterceptor }
  ]
})
export class RepostModule {}
