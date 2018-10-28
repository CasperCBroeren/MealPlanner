<template>
    <div>
        <nav-menu params="route: route"></nav-menu>
        <div id="app" class="container-fluid">
            <div class="row">
                <div class="col-sm-12">
                    <router-view></router-view>
                </div>
            </div>

        </div>
    </div>
</template>

<script>
    import Vue from 'vue'
    import Autocomplete from './autocomplete'
    import Meals from './meals'
    import Ingredients from './ingredients'
    import HomePage from './home-page'
    import WeekPlanning from './weekplanning.vue'
    import NavMenu from './nav-menu'


    Vue.component('autocomplete', Autocomplete);
    Vue.component('meals', Meals);
    Vue.component('ingredients', Ingredients);
    Vue.component('weekplanning', WeekPlanning);
    Vue.component('home-page', HomePage);
    Vue.component('nav-menu', NavMenu);

    export default {
        data() {
            return {
            }
        }
    }
</script>

<style>
</style>
