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

platformBrowserDynamic().bootstrapModule(AppModule);
