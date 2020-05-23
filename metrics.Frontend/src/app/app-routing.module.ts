import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {AccountComponent} from './components/account/account.component';
import {AuthenticatedGuard} from './helpers/authenticated.guard';
import {AppComponent} from './components/app/app.component';
import {AuthGuard} from './helpers/auth.guard';
import {UserProfileComponent} from './components/user-profile/user-profile.component';


const routes: Routes = [
  { path: 'login', component: AccountComponent, canActivate: [AuthenticatedGuard] },
  { path: 'profile', component: UserProfileComponent, canActivate: [AuthGuard] },
  { path: 'repost', canActivate: [AuthGuard], loadChildren: './modules/repost/repost.module#RepostModule' },
  { path: '*', component: AppComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
