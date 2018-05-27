import Meals from 'components/meals'
import HomePage from 'components/home-page'
import Ingredients from 'components/ingredients'

export const routes = [
    { path: '/', component: HomePage, display: 'Dashboard', style: 'glyphicon glyphicon-home' }, 
    { path: '/meals', component: Meals, display: 'Maaltijden', style: 'glyphicon glyphicon-th-list' },
    { path: '/ingredients', component: Ingredients, display: 'Ingrediënten', style: 'glyphicon glyphicon-grain' }
]