<template>
    <div class="container">
        <div class="row">
            <div class="col-12">
                <h2>Maaltijden</h2>

                <p>De maaltijden die we samen bedenken, gebruiken en opdienen. Ga er van uit dat het gerecht voor 4 personen wat betreft de hoeveelheden</p>
                <form v-on:submit.prevent="findMeal" v-if="!singleItem">

                    <button type="button" class="btn btn-primary" v-on:click="newMeal">Nieuw gerecht</button>
                </form>

                <form v-on:submit.prevent="stopSubmit" v-if="singleItem" class="">
                    <h3 v-if="editItem.created">Wijzig maaltijd</h3>
                    <h3 v-if="!editItem.created">Nieuwe maaltijd</h3>
                    <div class="form-group">
                        <label for="mealName">
                            Naam:
                        </label>
                        <input type="text" class="form-control" name="mealName" id="mealName" v-model="editItem.name" placeholder="kort en krachtig" autocomplete="off" />
                    </div>
                    <div class="form-group">

                        <label for="mealType">
                            Type:
                        </label>
                        <select type="text" class="form-control" name="mealType" id="mealType" v-model="editItem.mealType">
                            <option value="0" selected>Onbekend</option>
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
                        <autocomplete :items="tagsOptions" name="mealTags" id="mealTags"
                                      v-model="tagSearchFor"
                                      v-on:keydown-enter="addTag"
                                      v-on:lookup="lookupTags"
                                      placeholder=""
                                      isAsync />
                        <tagCollection :items="editItem.tags" />
                    </div>

                    <div class="form-group">

                        <label for="mealIngredients">
                            Ingrediënten:
                        </label>
                        <autocomplete :items="ingredientsOptions" name="mealIngredients" id="mealIngredients"
                                      v-on:keydown-enter="addIngredient"
                                      v-on:lookup="lookupIngredients"
                                      itemValueProperty="name"
                                       placeholder=""
                                      isAsync />
                        
                        <span v-if="ingredientToAdd">
                            Dit ingrediënt bestaat niet, wil je het toevoegen? <button v-on:click="registerNewIngredientAndAdd">Ja</button>
                            <button v-on:click="cancelNewIngredient">Nee</button>
                        </span>

                        <div class="ingredientsCollection" v-if="editItem.ingredients">
                            <span v-for="(item, index) in editItem.ingredients" v-bind:key="item.id">
                                <input type="text" v-model="item.amount" class="ingredientAmount" maxlength="50" />
                                {{ item.name }}
                                <a href="#" v-on:click="removeIngredient(index, $event)"> x </a>
                            </span>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="mealDescription">
                            Bereiding:
                        </label> <br />
                        <textarea class="form-control" name="mealDescription" id="mealDescription" v-model="editItem.description" rows="7" />
                    </div>
                    <button type="button" class="btn btn-primary" value="Opslaan" v-on:click="savemeal()">Opslaan</button>
                    <button type="button" class="btn" value="Terug" v-on:click="singleItem=false">Terug</button>

                    <button type="button" class="btn btn-danger float-right" value="Verwijder" v-if="editItem.created" v-on:click="deletemeal()">Verwijder</button>
                </form>
            </div>
        </div>

        <div class="row py-3">
            <div class="col-12">
                <table class="items table" v-if="meals && meals.length > 0 && !singleItem">
                    <thead>
                        <tr>
                            <th>Naam</th>
                            <th>Tags</th>
                            <th>Ingredienten</th>
                            <th>Aangemaakt op</th>
                            <th>Acties</th>
                        </tr>
                    </thead>
                    <tbody>

                        <tr v-for="item in meals" v-on:click="edit(item)" v-bind:key="item.id">
                            <td>{{ item.name }}</td>
                            <td>{{ renderTags(item.tags)}}</td>
                            <td>{{ renderIngredients(item.ingredients)}}</td>
                            <td>{{ simpleDate(item.created)}}</td>
                            <td>
                                <a href="#edit" class="btn btn-info" v-on:click="edit(item)">Bewerk</a>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>

</template>

<script>
    export default {
        data() {
            return {
                searchTerm: '',
                singleItem: false,
                tagSearchFor: null,
                editItem: {
                    name: "",
                    created: null,
                    ingredients: [],
                    tags: []
                },
                meals: null,
                ingredientSearchFor: null,
                ingredientToAdd: null,
                tagsOptions: [],
                ingredientsOptions: []
            }
        },
        filters: {

        },
        methods: {
            lookupIngredients: async function (val) {
                if (val.length > 0) {
                    let response = await this.$http.get('/api/Ingredients/search/' + encodeURI(val));
                    if (response.data != null) {
                        this.ingredientsOptions = response.data;
                    }
                }
                else {
                    this.ingredientsOptions = [];
                }
            },
            lookupTags: async function (val) {
                if (val.length > 0) {
                    let response = await this.$http.get('/api/Tag/Search/' + encodeURI(val));
                    if (response.data != null) {
                        this.tagsOptions = response.data;
                    }
                }
                else {
                    this.tagsOptions = [];
                }
            },
            simpleDate: function (date) {
                return this.$moment(date).format('LL');
            },
            filterValue: function (obj, key, value) {
                return obj.find(function (v) { return v[key] === value });
            },
            registerNewIngredientAndAdd: async function () {
                let response = await this.$http.post('/api/Ingredients/Save', this.ingredientToAdd);
                if (response.data != "nope") {
                    response.data.item.amount = 1;
                    this.editItem.ingredients.push(response.data.item);
                }
                this.ingredientToAdd = null;
            },
            cancelNewIngredient: function () {
                this.ingredientToAdd = null;
            },
            removeIngredient(i, ev) {

                this.editItem.ingredients.splice(i, 1);

            },
            incrementAmountIngredient(i, ev) {
                var item = this.editItem.ingredients[i];
                item.amount += 1;
                this.$set(this.editItem.ingredients, i, item);
            },
            removeTag: function (i, ev) {
                this.editItem.tags.splice(i, 1);
            },
            addTag: async function (value) {

                var tag = value.toUpperCase();
                if (this.editItem.tags.find(function (x) {
                    return tag == x.value;
                }) == null) {
                    this.editItem.tags.push({ id: null, value: tag });

                }
                this.tagSearchFor = null;

            },
            addIngredient: async function (value) {
                try {
                    let response = await this.$http.get('/api/Ingredients/Find/' + value);

                    if (response.data && this.filterValue(this.editItem.ingredients, "id", response.data.uid) == null) {
                        response.data.amount = 1;
                        this.editItem.ingredients.push(response.data);
                    }
                }
                catch (error) {

                    if (error.response != null && error.response.status == 404) {
                        this.ingredientToAdd = {
                            name: value,
                            amount: 1
                        }
                    }
                }

            },
            renderIngredients: function (items) {
                if (!items || items.length == 0) return "";
                var r = items[0].name;
                for (var i = 1; i < items.length; i++) {
                    r += ", " + items[i].name;
                }
                return r;
            },
            renderTags: function (items) {
                if (!items || items.length == 0) return "";
                var r = items[0].value;
                for (var i = 1; i < items.length; i++) {
                    r += ", " + items[i].value;
                }
                return r;
            },
            breakTags: function (val) {
                if (val == null) return [];
                if (Array.isArray(val)) return val;
                var result = [];
                val.split(',').forEach(function (p, i) {
                    result.push({
                        id: null,
                        value: p
                    });
                });
                return result;
            },
            cancel: function () {
                this.editItem = {
                    name: "",
                    created: null
                };
            },
            backToOverview: function () {
                this.singleItem = false;
                this.ingredientSearchFor = null;
            },
            edit: function (item) {
                this.singleItem = true;
                this.editItem = item;
            },
            newMeal: function () {
                this.cancel();
                this.singleItem = true;
                this.editItem = {
                    name: "",
                    created: null,
                    ingredients: [],
                    tags: []
                };
            },
            findMeal: async function (event) {

            },
            stopSubmit: function (event) {
                event.preventDefault();
                return false;
            },
            savemeal: async function (event) {
                if (this.editItem.name != "") {
                    this.editItem.tags = this.breakTags(this.editItem.tags);
                    let response = await this.$http.post('/api/Meal/Save', this.editItem);
                    if (response.data != "nope") {
                        if (response.data.created != this.editItem.created && response.data.isDuplicate == false) {
                            this.editItem.created = response.data.created;
                            this.meals.push(this.editItem);
                        }
                    }
                    this.backToOverview();
                }
            },
            deletemeal: async function (event) {
                let response = await this.$http.post('/api/Meal/Delete', this.editItem);

                if (response.data == "done") {
                    this.meals.splice(this.meals.indexOf(this.editItem), 1);
                    this.singleItem = false;
                }
                this.backToOverview();
            }
        },

        async created() {

            try {
                let response = await this.$http.get('/api/Meal/All')
                this.meals = response.data;
            } catch (error) {
                console.log(error)
            }

        }
    }
</script>

<style>
    .items {
        list-style-type: none;
    }

        .items li {
            display: inline-block;
            background-color: #aaa;
            padding: 0.25% 0.5%;
            margin: 0.25% 0.5%;
            font-variant: small-caps;
            cursor: pointer;
        }
</style>
