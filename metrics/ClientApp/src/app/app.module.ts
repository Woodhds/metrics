import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { ProductListComponent } from './Product/product-list/product-list.component';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {HttpCorsInterceptor} from "./http-interceptor";


@NgModule({
  declarations: [
    AppComponent,
    ProductListComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'metrics' }),
    AppRoutingModule,
    GridModule,
    BrowserAnimationsModule,
    HttpClientModule
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: HttpCorsInterceptor, multi: true}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
