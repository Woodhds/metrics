import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {RouterModule, Routes} from "@angular/router";
import {AuthGuard} from "../../helpers/auth.guard";
import {MatInputModule} from "@angular/material/input";
import {MatAutocompleteModule} from "@angular/material/autocomplete";
import {RepostComponent} from "./components/repost/repost.component";
import {HTTP_INTERCEPTORS} from "@angular/common/http";
import {AuthInterceptor} from "../../http-interceptors/auth-interceptor";
import {ReactiveFormsModule} from "@angular/forms";
import { UserComponent } from './components/user/user.component';

const routes: Routes = [
  { path: '', canActivate: [AuthGuard], component: RepostComponent },
  { path: 'user', canActivate: [AuthGuard], component: UserComponent }
];

@NgModule({
  declarations: [RepostComponent, UserComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    MatInputModule,
    MatAutocompleteModule,
    ReactiveFormsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, multi: true, useClass: AuthInterceptor }
  ]
})
export class RepostModule { }
