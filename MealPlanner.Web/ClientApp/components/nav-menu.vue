<template>
    <nav class="navbar navbar-expand-lg navbar-light bg-light mb-3">
        <a class="navbar-brand" href="/"><b>Maaltijd</b>Plan</a>
        <i>{{this.getGroupName()}}</i>
        <ul class="navbar-nav mr-auto">
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
                routes,
                collapsed: false
            }
        },
        methods: {
            toggleCollapsed: function (event) {
                this.collapsed = !this.collapsed;
            },
            getGroupName: function () {
                return this.$cookies.get('mpggn');
            }
        }
    }
</script>

<style>
    .navbar i { 
    }
    .slide-enter-active, .slide-leave-active {
        transition: max-height .35s
    }

    .slide-enter, .slide-leave-to {
        max-height: 0px;
    }

    .slide-enter-to, .slide-leave {
        max-height: 20em;
    }
</style>
