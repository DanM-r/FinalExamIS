import { createApp } from 'vue'
import App from './App.vue'

import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';

import bootstrap from 'bootstrap';

import '../scss/custom.css';

const app = createApp(App);

app.use(bootstrap);
app.mount('#app')
