import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex) 

export default new Vuex.Store({
    state: {
        jwtToken: null,
        groupName: null
    },
    mutations: {
        login(state, payload) { 
            state.jwtToken = payload.jwt;
            state.groupName = payload.name;
            localStorage.setItem('jwtToken', payload.jwt);
            localStorage.setItem('groupName', payload.name);
        },
        logout(state) {
            state.jwtToken = null;
            state.groupName = null;

            localStorage.removeItem('jwtToken');
            localStorage.removeItem('groupName');
        },
        initialiseStore(state) {
             
            if (!state.jwtToken &&
                localStorage.getItem('jwtToken') &&
                localStorage.getItem('groupName')) {
                
                state.jwtToken = localStorage.getItem('jwtToken');
                state.groupName = localStorage.getItem('groupName'); 
            }
        }
    }
});

Date.prototype.getWeek = function () {
    var date = new Date(this.getTime());
    date.setHours(0, 0, 0, 0);
    // Thursday in current week decides the year.
    date.setDate(date.getDate() + 3 - (date.getDay() + 6) % 7);
    // January 4 is always in week 1.
    var week1 = new Date(date.getFullYear(), 0, 4);
    // Adjust to Thursday in week 1 and count number of weeks from date to week1.
    return 1 + Math.round(((date.getTime() - week1.getTime()) / 86400000
        - 3 + (week1.getDay() + 6) % 7) / 7);
}
