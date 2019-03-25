import Meals from 'components/meals'

import WeekPlanning from 'components/weekplanning'
import HomePage from 'components/home-page'
import Ingredients from 'components/ingredients'
import Shoppinglist from './components/shoppinglist.vue';
import Recipe from './components/recipe.vue';
import Options from './components/options.vue';
import Login from './components/login.vue';
import NewGroup from './components/newgroup.vue';

export const routes = [
    { path: '/', component: HomePage, display: 'Dashboard', style: 'fa fa-home', meta: { public: false, hideMenu: false, hideInMenu: true } },
    {
        path: '/weekplanning', component: WeekPlanning, display: 'Weekplan', style: 'fa fa-list-ul', meta: { public: false, hideMenu: false },
        children: [
            {
                path: ':year/:week', component: WeekPlanning
            }
        ]
    },
    { path: '/meals', component: Meals, display: 'Maaltijden', style: 'fa fa-utensils', meta: { public: false, hideMenu: false } },
    { path: '/ingredients', component: Ingredients, display: 'Ingrediënten', style: 'fa fa-lemon', meta: { public: false, hideMenu: false } },
    { path: '/options', component: Options, display: 'Opties', style: 'fa fa-cog', meta: { public: false, hideMenu: false } },
    { path: '/login', component: Login,  meta: { public: true, hideMenu: true } },
    { path: '/logout', component: Login, meta: { public: true, hideMenu: true } },
    { path: '/new', component: NewGroup,  meta: { public: true, hideMenu: true } },
    {
        path: '/shoppinglist/:year/:week', component: Shoppinglist,  meta: { public: false, hideMenu: false, hideInMenu: true  }, props: (route) => ({
            year: route.params.year,
            week: route.params.week
        })
    },
    {
        path: '/meal/:id/:year/:week', component: Recipe, meta: { public: false, hideMenu: false  }, props: (route) => ({
            id: route.params.id,
            year: route.params.year,
            week: route.params.week
        })
    }
]
