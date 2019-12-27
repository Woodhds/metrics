import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./components/app/app.component";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import { AuthGuard } from "./helpers/auth.guard";
import { IAuthService } from "./services/abstract/IAuth";
import { AuthService } from "./services/concrete/auth/auth.service";
import { LoginComponent } from "./components/login/login.component";
import { NavBarComponent } from "./components/nav-bar/nav-bar.component";
import { AccountComponent } from "./components/account/account.component";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatButtonModule } from "@angular/material/button";
import { MatToolbarModule } from "@angular/material/toolbar";
import { HttpClientModule } from "@angular/common/http";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { ReactiveFormsModule } from "@angular/forms";
import {IAppConfigService} from "./services/abstract/IAppConfigService";
import {AppConfigService} from "./services/concrete/AppConfig/AppConfigService";

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    NavBarComponent,
    AccountComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    CommonModule,
    MatIconModule,
    BrowserAnimationsModule,
    MatToolbarModule,
    HttpClientModule,
    MatButtonModule,
    MatInputModule,
    ReactiveFormsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthGuard, multi: true },
    { provide: IAuthService, useClass: AuthService },
    { provide: IAppConfigService, useClass: AppConfigService }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
