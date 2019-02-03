import Vue from 'vue' 
import axios from 'axios'
import moment from 'moment'
import router from './router'
import store from './store'
import { sync } from 'vuex-router-sync'
import App from 'components/app-root'
import VueAppInsights from 'vue-application-insights'
import VueCookies from 'vue-cookies'

Vue.use(VueCookies);
 
Vue.use(VueAppInsights, {
    id: '2b8c5f23-23fa-4234-90f6-11de0acd37b4',
    router
});
Vue.prototype.$http = axios;
Vue.prototype.$moment = moment;

sync(store, router)

const app = new Vue({
    store,
    router,
    ...App
})

export {
    app,
    router,
    store
}
