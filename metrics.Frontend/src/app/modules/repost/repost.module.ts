import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '../../helpers/auth.guard';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { RepostComponent } from './components/repost/repost.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { AuthInterceptor } from '../../http-interceptors/auth-interceptor';
import { AddUserComponent } from './components/add-user/add-user.component';
import { UserComponent } from './components/user/user.component';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatIconModule } from '@angular/material/icon';
import { VkImageComponent } from './components/vk-image/vk-image.component';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSliderModule } from '@angular/material/slider';
import { ReactiveFormsModule } from '@angular/forms';
import { LazyImageDirective } from '../../directives/lazy-image/lazy-image.directive';
import { MessageComponent } from './components/message/message.component';
import { MatTableModule } from '@angular/material/table'

const routes: Routes = [
  { path: '', canActivate: [AuthGuard], component: RepostComponent },
  { path: 'add', canActivate: [AuthGuard], component: AddUserComponent },
  { path: 'user', canActivate: [AuthGuard], component: UserComponent },
  { path: 'message', canActivate: [AuthGuard], component: MessageComponent }
];

@NgModule({
  declarations: [
    RepostComponent,
    AddUserComponent,
    UserComponent,
    VkImageComponent,
    LazyImageDirective,
    MessageComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    MatInputModule,
    MatAutocompleteModule,
    ReactiveFormsModule,
    MatButtonModule,
    HttpClientModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatPaginatorModule,
    MatIconModule,
    MatSlideToggleModule,
    MatSliderModule,
    MatTableModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, multi: true, useClass: AuthInterceptor }
  ]
})
export class RepostModule {}
