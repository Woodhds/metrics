import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {RepostComponent} from "./repost/repost.component";
import {RouterModule, Routes} from "@angular/router";
import {AuthGuard} from "../../helpers/auth.guard";

const routes: Routes = [
  { path: '', canActivate: [AuthGuard], component: RepostComponent }
];

@NgModule({
  declarations: [RepostComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ]
})
export class RepostModule { }
