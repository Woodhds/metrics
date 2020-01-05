import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {AccountComponent} from "./components/account/account.component";


const routes: Routes = [
  { path: 'login', component: AccountComponent },
  { path: 'repost', loadChildren: './modules/repost/repost.module#RepostModule' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
