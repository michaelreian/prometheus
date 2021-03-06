import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpModule, Http, RequestOptions } from '@angular/http';
import { RouterModule, Routes } from '@angular/router';

import { AuthHttp, AuthConfig } from 'angular2-jwt';
import { BsDropdownModule } from 'ngx-bootstrap';

import { AppComponent } from './app.component';
import { TopBarComponent } from './top-bar/top-bar.component';
import { SideBarComponent } from './side-bar/side-bar.component';
import { FooterComponent } from './footer/footer.component';
import { HomeComponent } from './home/home.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';

import { PicaroonComponent } from './picaroon/picaroon.component';
import { PicaroonService } from './picaroon/picaroon.service';
import { PicaroonSearchComponent } from './picaroon/picaroon-search.component';
import { PicaroonSearchBarComponent } from './picaroon/picaroon-search-bar.component';
import { PicaroonBrowseComponent } from './picaroon/picaroon-browse.component';
import { PicaroonDetailComponent } from './picaroon/picaroon-detail.component';
import { SafeUrlPipe } from './safe-url.pipe';
import { CallbackComponent } from './callback/callback.component';

import { AuthService } from './auth.service';

const appRoutes: Routes = [
  {
    path: 'home',
    component: HomeComponent
  },
  {
    path: 'picaroon',
    component: PicaroonComponent
  },
  {
    path: 'picaroon/browse',
    component: PicaroonSearchComponent
  },
  {
    path: 'picaroon/search',
    component: PicaroonSearchComponent
  },
  {
    path: 'picaroon/detail/:id',
    component: PicaroonDetailComponent
  },
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },
  { path: '**', component: PageNotFoundComponent }
];

export function authHttpServiceFactory(http: Http, options: RequestOptions) {
  return new AuthHttp(new AuthConfig({
    tokenGetter: (() => localStorage.getItem('access_token'))
  }), http, options);
}


@NgModule({
  declarations: [
    AppComponent,
    TopBarComponent,
    SideBarComponent,
    FooterComponent,
    HomeComponent,
    PageNotFoundComponent,
    PicaroonComponent,
    PicaroonSearchComponent,
    PicaroonSearchBarComponent,
    PicaroonBrowseComponent,
    PicaroonDetailComponent,
    SafeUrlPipe,
    CallbackComponent
  ],
  imports: [
    BsDropdownModule.forRoot(),
    RouterModule.forRoot(appRoutes),
    BrowserModule,
    FormsModule,
    HttpModule
  ],
  providers: [AuthService, PicaroonService, {
      provide: AuthHttp,
      useFactory: authHttpServiceFactory,
      deps: [Http, RequestOptions]
    }],
  bootstrap: [AppComponent]
})
export class AppModule { }
