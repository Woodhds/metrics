import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {AccountComponent} from "./components/account/account.component";
import {AuthenticatedGuard} from "./helpers/authenticated.guard";
import {AppComponent} from "./components/app/app.component";


const routes: Routes = [
  { path: 'login', component: AccountComponent, canActivate: [AuthenticatedGuard] },
  { path: 'repost', loadChildren: './modules/repost/repost.module#RepostModule' },
  { path: '*', component: AppComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
