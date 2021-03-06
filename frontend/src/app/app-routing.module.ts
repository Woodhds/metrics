import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccountComponent } from './components/account/account.component';
import { AuthenticatedGuard } from './helpers/authenticated.guard';
import { AppComponent } from './components/app/app.component';
import { AuthGuard } from './helpers/auth.guard';

const routes: Routes = [
  {
    path: 'login',
    component: AccountComponent,
    canActivate: [AuthenticatedGuard],
  },
  {
    path: 'profile',
    canActivate: [AuthGuard],
    loadChildren: './modules/profile/profile.module#ProfileModule',
  },
  {
    path: 'repost',
    canActivate: [AuthGuard],
    loadChildren: './modules/repost/repost.module#RepostModule',
  },
  { path: '*', component: AppComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
