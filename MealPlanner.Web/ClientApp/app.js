import Vue from 'vue' 
import axios from 'axios'
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
