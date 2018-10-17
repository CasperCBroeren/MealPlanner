<template>
    <div class="container">
        <div class="row">
            <div class="col-sm">
                <h1>Maaltijden</h1>

                <p>De maaltijden die we samen bedenken, gebruiken en opdienen.</p>
                <form v-on:submit.prevent="findMeal" v-if="!singleItem">
                    <label for="mealName">
                        Zoek maaltijd:
                    </label>
                    <input type="text" name="SearchTerm" id="searchTerm" v-model="searchTerm" placeholder="naam, tags of ingredienten" autocomplete="off" />
                    <br />
                    <input type="button" value="Nieuw gerecht" v-on:click="newMeal" />
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
                                      isAsync />

                        <ul class="tagCollection" v-if="editItem.tags">
                            <li v-for="(item, index) in editItem.tags" v-bind:key="item.id">
                                {{ item.value }}
                                <a v-on:click="removeTag(index, $event)"> x </a>
                            </li>
                        </ul>
                    </div>

                    <div class="form-group">

                        <label for="mealIngredients">
                            Ingrediënten:
                        </label>
                        <input type="text" class="form-control" name="mealIngredients" id="mealIngredients" v-model="ingredientSearchFor" autocomplete="off" v-on:keydown="addIngredient" />
                        <span v-if="ingredientToAdd">
                            Dit ingrediënt bestaat niet, wil je het toevoegen? <button v-on:click="registerNewIngredientAndAdd">Ok</button>
                        </span>

                        <ul class="ingredientsCollection" v-if="editItem.ingredients">
                            <li v-for="(item, index) in editItem.ingredients" v-bind:key="item.id">
                                <a v-on:click="decrementAmountIngredient(index, $event)"> - </a>
                                {{ item.name }}
                                <span v-if="item.amount > 1">
                                    x  {{item.amount}}
                                </span>
                                <a v-on:click="incrementAmountIngredient(index, $event)"> + </a>

                            </li>
                        </ul>
                    </div>
                    <div class="form-group">
                        <label for="mealDescription">
                            Beschrijving:
                        </label> <br />
                        <textarea class="form-control" name="mealDescription" id="mealDescription" v-model="editItem.description" />
                    </div>
                    <input type="button" value="Opslaan" v-on:click="savemeal()" />
                    <input type="button" value="Verwijder" v-if="editItem.created" v-on:click="deletemeal()" />
                    <input type="button" value="Terug" v-if="editItem.created" v-on:click="singleItem=false" />
                </form>

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
                                <a href="#edit" v-on:click="edit(item)">Bewerk</a>
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
                    ingredientsAmount: []
                },
                meals: null,
                ingredientSearchFor: null,
                ingredientToAdd: null,
                tagsOptions: []
            }
        },
        filters: {

        },
        methods: {
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
            simpleDate: function (date) {
                return this.$moment(date).format('LL');
            },
            filterValue: function (obj, key, value) {
                return obj.find(function (v) { return v[key] === value });
            },
            registerNewIngredientAndAdd: async function () {
                let response = await this.$http.post('/api/Ingredients/Save', this.ingredientToAdd);
                if (response.data != "nope") {
                    this.editItem.ingredients.push(response.data.item);
                }
                this.ingredientToAdd = null;
            },
            decrementAmountIngredient(i, ev) {
                var item = this.editItem.ingredients[i];
                item.amount -= 1;

                if (this.editItem.ingredients[i].amount > 1) {
                    this.$set(this.editItem.indgredients, i, item);
                }
                else {
                    this.editItem.ingredients.splice(i, 1);
                }
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
            addIngredient: async function (ev) {
                this.ingredientToAdd = null;
                if (ev.keyCode == 13) {
                    try {
                        let response = await this.$http.get('/api/Ingredients/Find/' + this.ingredientSearchFor);

                        if (response.data && this.filterValue(this.editItem.ingredients, "id", response.data.uid) == null) {
                            response.data.amount = 1;
                            this.editItem.ingredients.push(response.data);
                        }
                        this.ingredientSearchFor = null;
                    }
                    catch (error) {

                        if (error.response != null && error.response.status == 404) {
                            this.ingredientToAdd = {
                                name: this.ingredientSearchFor
                            }
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
                    ingredientsAmount: []
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
            padding: 0.25% 2.5%;
            margin: 0.25% 0.5%;
            font-variant: small-caps;
            cursor: pointer;
        }
</style>
