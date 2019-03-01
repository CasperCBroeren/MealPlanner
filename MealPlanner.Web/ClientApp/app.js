import Vue from 'vue' 
import axios from 'axios'
import wrapper from 'axios-cache-plugin'
import moment, { locale } from 'moment'
import router from './router'
import store from './store'
import { sync } from 'vuex-router-sync'
import App from 'components/app-root'
import VueAppInsights from 'vue-application-insights'
 
 
Vue.use(VueAppInsights, {
    id: '2b8c5f23-23fa-4234-90f6-11de0acd37b4',
    router
});

axios.interceptors.request.use(function (request) { 
    request.headers.authorization = 'Bearer ' + store.state.jwtToken;
    return request;
});

axios.interceptors.response.use(function (response) {
        return response;
    },
    function (error) {
        if (error.response.status === 401) { 
            store.commit('logout');
            router.push("/login");
        } 
        return error;
    });

Vue.prototype.$http = wrapper(axios, {
    maxCacheSize: 15,
    ttl: 1000 * 60 * 60 * 24 * 10
});
Vue.prototype.$http.__addFilter(/Ingredients/);
Vue.prototype.$http.__addFilter(/Meal/);
Vue.prototype.$http.__addFilter(/ShoppingList/);
Vue.prototype.$http.__addFilter(/Weekplanning/);


Vue.prototype.$moment = moment;

sync(store, router)

const app = new Vue({
    store,
    router,
    ...App
});

(function () {
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.register('./service-worker.js')
            .then(() => console.log('Service Worker registered successfully.'))
            .catch(error => console.log('Service Worker registration failed:', error));
    }
})();

export {
    app,
    router,
    store
}


// IOS install popup 
const isIos = () => {
    const userAgent = window.navigator.userAgent.toLowerCase();
    return /iphone|ipad|ipod/.test(userAgent);
}
 const isInStandaloneMode = () => ('standalone' in window.navigator) && (window.navigator.standalone);
 
if (isIos() && !isInStandaloneMode()) {
    this.setState({ showInstallMessage: true });
}
