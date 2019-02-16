import Vue from 'vue'
import VueRouter from 'vue-router'

import { routes } from './routes'

Vue.use(VueRouter);

let router = new VueRouter({
    mode: 'history',
    routes
})

router.beforeEach((to, from, next) => {

    var token = localStorage.getItem("jwtToken");
    // todo validate token on server
    if (token != null
        || to.matched.some(record => record.meta.public)) {
        next();
    }
    else {
        next({
            path: 'login',
            params: { nextUrl: to.fullPath }
        });
    }
})

export default router
