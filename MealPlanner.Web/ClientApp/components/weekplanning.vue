<template>
    <div class="container">
        <div class="row">
            <div class="col-sm">
                <h1>Week planning</h1>
                <p>Plan een week met maaltijden en krijg een handige uitdraai van alle ingredienten</p>
                <nav aria-label="Week">
                    <ul class="pagination justify-content-center">
                        <li class="page-item"><a class="page-link" :href="'/weekplanning/' + prevYear + '/' + prevWeek">&lt;&lt;</a></li>
                        <li class="page-item disabled"><a class="page-link pl-5 pr-5" href="#"> Week <b>{{week}}</b> van <b>{{year}}</b></a></li>
                        <li class="page-item "><a class="page-link" :href="'/weekplanning/' + nextYear + '/' + nextWeek">&gt;&gt;</a></li>
                    </ul>
                </nav>
                <div class="card-group">
                    <div v-for="day in meals" class="card" style="width:18rem">
                        <div class="card-header">
                            {{day.dayName}}
                        </div>
                        <div class="card-body"  v-on:click="startMealSelection(day)">
                            <p v-if="day.meal" class="card-text">{{day.meal.name}}</p>
                            <p v-else class="card-text"><small class="text-muted">Deze dag is nog niet ingevuld</small></p>
                        </div>
                        <div class="card-footer">
                            <button type="button" class="btn btn-info" v-on:click="startMealSelection(day)">Kies</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal fade show" tabindex="-1" role="dialog" v-if="decideMealForDay">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Kies een maaltijd voor {{decideMealForDay.day}}</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close" v-on:click="stopMealSelection()">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body text-center" v-if="questionType == 0">
                        Selecteer maaltijden per<br />
                        <button type="button" class="btn btn-info btn-block mt-2" v-on:click="questionType=1">
                            <span class="fas fa-utensils"></span> Maaltijd
                        </button>
                        <button type="button" class="btn btn-info btn-block" v-on:click="questionType=2">
                            <span class="fas fa-lemon"></span>  Ingredienten
                        </button>

                        <button type="button" class="btn btn-info btn-block" v-on:click="questionType=3">
                            <span class="fas fa-tags"></span> Type of Tag
                        </button>
                    </div>
                    <div class="modal-body" v-if=" questionType==1">
                        <div class="form-group">
                            <label for="searchForMeal">Zoek maaltijd</label>
                            <input type="text" class="form-control" v-model="searchForMeal" id="searchForMeal" placeholder="Naam van maaltijd" v-on:keydown.enter="searchMeal()">
                        </div>
                    </div>
                    <div class="modal-body" v-if="questionType ==2">
                        <div class="form-group">
                            <label for="searchForIngredient">Zoek ingredient</label>
                            <autocomplete name="mealIngredients" id="mealIngredients"
                                          :items="ingredientOptions"
                                          v-on:keydown-enter="addIngredient"
                                          v-on:lookup="lookupIngredients"
                                          itemValueProperty="name"
                                          isAsync />
                            <tagCollection ref="searchForIngredients" :items="searchForIngredients" itemLabelProperty="name" :onItemRemoved="findMealsByIngredients" />
                        </div>
                    </div>
                    <div class="modal-body" v-if="questionType ==3">
                        <div class="form-group">

                            <label for="mealType">
                                Zoek op type:
                            </label>
                            <select type="text" class="form-control" name="mealType" id="mealType" v-model="searchForMealType">
                                <option value="0" selected>Allemaal</option>
                                <option value="1">Vlees</option>
                                <option value="2">Vis</option>
                                <option value="4">Vegatarisch</option>
                                <option value="5">Zoet</option>
                            </select>
                        </div>

                        <div class="form-group">

                            <label for="mealTags">
                                Tags:
                            </label>
                            <autocomplete name="mealTags" id="mealTags"
                                          v-model="tagSearchFor"
                                          v-on:keydown-enter="addTag"
                                          v-on:lookup="lookupTags"
                                          isAsync />

                            <ul class="tagCollection" v-if="searchForTags">
                                <li v-for="(item, index) in searchForTags" v-bind:key="item.id">
                                    {{ item.value }}
                                    <a v-on:click="removeTag(index, $event)"> x </a>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <div class="modal-body">
                        <div class="results" v-if="questionType > 0">
                            <ul class="list-group">
                                <a href="#" v-for="meal in mealResults" v-on:click="selectMeal(meal)" class="list-group-item list-group-item-action flex-column align-items-start">
                                    <div class="d-flex w-100 justify-content-between">
                                        <h5 class="mb-1">{{meal.name}}</h5>
                                        <small>{{meal.created | formatDate}}</small>
                                    </div>
                                    <small>
                                        <span class="ingredientSmall" v-for="ingredient in meal.ingredients">{{ingredient.name}}</span>
                                    </small>
                                </a>

                            </ul>
                        </div>
                    </div>
                    <div class="modal-footer" v-if="questionType >0">
                        <small class="text-muted" v-if="propesedMeal">Geselecteerd: {{propesedMeal.name}}</small>

                        <button type="button" class="btn btn-secondary" v-on:click="back()">Terug</button>
                        <button type="button" class="btn btn-primary" v-bind:class="{ disabled: !propesedMeal}" v-on:click="finishSelectionMeal()">Selecteer</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
   
</template>

<script>
    import moment from 'moment'

    export default {
        props: [
            'week',
            'year'
        ],
        filters: {
            formatDate: function (value) {
                if (value) {
                    return moment(String(value)).format('MM/DD/YYYY hh:mm')
                }
            }
        },
        data() {
            return {
                questionType: 0,
                searchForMeal: null,
                searchForIngredients: [],
                searchForMealType: 0,
                searchForTags: [],
                mealResults: [],
                decideMealForDay: null,
                propesedMeal: null,
                ingredientOptions: [],
                step: 1,
                weekPlanningId: null,
                meals: [
                    { dayName: 'maandag', meal: null },
                    { dayName: 'dinsdag', meal: null },
                    { dayName: 'woensdag', meal: null },
                    { dayName: 'donderdag', meal: null },
                    { dayName: 'vrijdag', meal: null },
                    { dayName: 'zaterdag', meal: null },
                    { dayName: 'zondag', meal: null },
                ],
                tagSearchFor: null,
                tagOptions: [],
                tags: []
            }
        },
        computed: {
            prevYear: function () {
                if (this.week > 1)
                    return this.year;
                else {
                    return this.year - 1;
                }
            },
            nextYear: function () {
                if (this.week < 51)
                    return this.year;
                else {
                    return this.year + 1;
                }
            },
            prevWeek: function () {
                if (this.week == 1)
                    return 51;
                else {
                    return this.week - 1;
                }
            },
            nextWeek: function () {
                if (this.week >= 51)
                    return 1;
                else {
                    return this.week + 1;
                }
            }
        },
        methods: {
            lookupIngredients: async function (val) {
                if (val.length > 0) {
                    let response = await this.$http.get('/api/Ingredients/search/' + encodeURI(val));
                    if (response.data != null) {
                        this.ingredientOptions = response.data;
                    }
                }
                else {
                    this.ingredientOptions = [];
                }
            },
            lookupTags: async function (val) {
                if (val.length > 0) {
                    let response = await this.$http.get('/api/Tag/find/' + encodeURI(val));
                    if (response.data != null) {
                        this.tagsOptions = response.data;
                    }
                }
                else {
                    this.tagsOptions = [];
                }
            },
            addTag: async function (value) {

                var tag = value.toUpperCase();
                if (this.searchForTags.find(function (x) {
                    return tag == x.value;
                }) == null) {
                    this.searchForTags.push({ id: null, value: tag });

                }
                this.tagSearchFor = null;

            },
            filterValue: function (obj, key, value) {
                return obj.find(function (v) { return v[key] === value });
            },
            addIngredient: async function (value) {
                try {
                    let response = await this.$http.get('/api/Ingredients/Find/' + value); 
                    this.$refs.searchForIngredients.add(response.data);
                    this.findMealsByIngredients();
                }
                catch (error) {

                    if (error.response != null && error.response.status == 404) {
                      
                    }
                }

            },
            findMealsByIngredients: async function () {
                try {
                    let response = await this.$http.post('/api/Meal/FindByIngredients', this.searchForIngredients);

                    this.mealResults = response.data;

                }
                catch (error) {

                }
            },
            back: function () {
                this.startMealSelection(this.decideMealForDay);
            },
            startMealSelection: function (day) {
                this.questionType = 0;
                this.step = 1;
                this.searchForMeal = null;
                this.decideMealForDay = day;
                this.propesedMeal = null;
                this.mealResults = [];
                this.searchForIngredients = [];
            },
            stopMealSelection: function () {
                this.propesedMeal = null;
                this.decideMealForDay = null;

                this.propesedMeal = null;
                this.mealResults = [];
            },
            selectMeal: function (meal) {
                this.propesedMeal = meal;
            },
            finishSelectionMeal: function () {
                this.decideMealForDay.meal = this.propesedMeal; 
                this.stopMealSelection();
                this.saveWeekplan();
            },
            saveWeekplan: async function () {
                try {
                    let response = await this.$http.post('/api/Weekplanning/Save', {
                        days: this.meals,
                        week: this.week,
                        year: this.year,
                        id: this.weekPlanningId
                    });
                    this.weekPlanningId = response.data.id;
                }
                catch (error) {

                    if (error.response != null && error.response.status == 404) {

                    }
                }
            },
            searchMeal: async function() {
                try {
                    let response = await this.$http.get('/api/Meal/Find/' + encodeURI(this.searchForMeal));

                    if (response.data) {
                        this.mealResults = response.data;
                    }
                }
                catch (error) {

                    if (error.response != null && error.response.status == 404) {
                        
                    }
                }
            }
            
        },

        async created() { 
            try {
                let response = await this.$http.get('/api/Weekplanning/' + this.year + '/' + this.week);

                if (response.data) {
                    if (response.data.days != null) {
                        this.meals = response.data.days;
                    }
                    this.weekPlanningId = response.data.id;
                }
            }
            catch (error) {

                if (error.response != null && error.response.status == 404) {

                }
            }
            
        }
    }
</script>
<style>
    .page-link {
        color: #6c757d; 
    }
    .modal-dialog {
        overflow-y: initial !important
    }

    .results {
        height: 250px;
        overflow-y: auto;
    }
    .ingredientSmall::before {
        content: ' - ';
    }
    .ingredientSmall:first-child::before {
        content: '';
    }
</style>
