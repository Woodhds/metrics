import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { AppRoutingModule } from "./app-routing.module";
import { RepostModule } from "./modules/repost/repost.module";
import { AppComponent } from "./components/app/app.component";
import { IAuthService } from "./services/abstract/IAuth";
import { AuthService } from "./services/concrete/auth/auth.service";
import { LoginComponent } from "./components/login/login.component";
import { NavBarComponent } from "./components/nav-bar/nav-bar.component";
import { AccountComponent } from "./components/account/account.component";
import { MatIconModule } from "@angular/material/icon";
import {MatMenuModule} from '@angular/material/menu';
import { OverlayModule } from "@angular/cdk/overlay";
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
    ReactiveFormsModule,
    RepostModule,
    MatMenuModule,
    OverlayModule
  ],
  providers: [
    { provide: IAuthService, useClass: AuthService },
    { provide: IAppConfigService, useClass: AppConfigService }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
