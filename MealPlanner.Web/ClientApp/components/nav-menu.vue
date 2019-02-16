<template>
    <nav class="navbar navbar-expand-lg navbar-light bg-light mb-3">
        <img src="/favicon.png" class="img-fluid mr-3" />
        <a class="navbar-brand" href="/"><b>Maaltijd</b>Plan</a>
        <i  v-if="this.showMenu()"> 
             {{this.getGroupName()}}</i>
        <ul class="navbar-nav mr-auto" v-if="this.showMenu()">
            <li v-for="route in routes" class="nav-item ml-3"> 
                <router-link v-if="!route.hideInMenu" :to="route.path" class="nav-link">
                    <span :class="route.style"></span> {{ route.display }}
                </router-link>
            </li>
        </ul>

    </nav>
</template>

<script>
    import { routes } from '../routes'

    export default {
        data() {
            return {
                routes
            }
        },
        methods: {
            showMenu: function () {
                var $route = this.$route;
                var theRoute = this.routes.find(function (x) { return x.path.toLowerCase().indexOf($route.path.toLowerCase()) > -1 });
                
                return theRoute.meta.hideMenu ? false : true;
            },
            toggleCollapsed: function (event) {
                this.collapsed = !this.collapsed;
            },
            getGroupName: function () {
                return localStorage.getItem('groupName');
            }
        }
    }
</script>
