import * as L from 'leaflet';

const win = window as any;
if (!win.L) {
  win.L = L;
}

import 'leaflet-routing-machine';
