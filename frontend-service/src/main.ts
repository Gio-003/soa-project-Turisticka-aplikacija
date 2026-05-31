import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import './app/shared/map/leaflet-setup';

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));
