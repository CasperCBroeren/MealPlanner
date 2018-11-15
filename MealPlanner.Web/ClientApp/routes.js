import Meals from 'components/meals'

import WeekPlanning from 'components/weekplanning'
import HomePage from 'components/home-page'
import Ingredients from 'components/ingredients'

export const routes = [
    { path: '/', component: HomePage, display: 'Dashboard', style: 'fa fa-home', hideInMenu: true },
    {
        path: '/weekplanning', component: WeekPlanning, display: 'Weekplan', style: 'fa fa-list-ul',props:
            (route) => ({
                year: route.params.year == null ? new Date().getFullYear() : parseInt(route.params.year),
            week: route.params.week == null? new Date().getWeek() : parseInt(route.params.week)
        }),
        children: [
            {
                path: ':year/:week', component: WeekPlanning
            }
        ]
    },
    { path: '/meals', component: Meals, display: 'Maaltijden', style: 'fa fa-utensils' },
    { path: '/ingredients', component: Ingredients, display: 'Ingrediënten', style: 'fa fa-lemon' }
]
