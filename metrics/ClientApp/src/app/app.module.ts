import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {NavbarComponent} from './navbar/navbar.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {ApiInterceptor} from './services/apiinterceptor';
import { UserComponent } from './user/user.component';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import {FormsModule} from '@angular/forms';
import { UnixtimePipe } from './core/pipes/unixtime/unixtime.pipe';
import { IntlModule } from '@progress/kendo-angular-intl';
import { ToolBarModule } from '@progress/kendo-angular-toolbar';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { MessageImageComponent } from './message-image/message-image.component';
import { CommonModule } from '@angular/common';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { EntitiesComponent } from './entities/entities.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { AddentityComponent } from './addentity/addentity.component';
import { DialogsModule } from '@progress/kendo-angular-dialog';



@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    UserComponent,
    LoginComponent,
    HomeComponent,
    UnixtimePipe,
    MessageImageComponent,
    EntitiesComponent,
    PageNotFoundComponent,
    AddentityComponent
  ],
  imports: [
    GridModule,
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    CommonModule,
    BrowserAnimationsModule,
    FormsModule,
    IntlModule,
    InputsModule,
    ToolBarModule,
    ButtonsModule,
    TooltipModule,
    DialogsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ApiInterceptor, multi: true }
  ],
  entryComponents: [AddentityComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
