import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import axios from 'axios';
import 'metismenu';
import 'pace-js';

import './polyfills';
import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

require('./styles.scss');

if (environment.production) {
  enableProdMode();
}

axios.interceptors.request.use(function(config) {
  if(localStorage.access_token) {
    config.headers.Authorization = 'Bearer ' + localStorage.access_token;
  }
  return config;
}, function(error) {
  return Promise.reject(error);
});

platformBrowserDynamic().bootstrapModule(AppModule);
