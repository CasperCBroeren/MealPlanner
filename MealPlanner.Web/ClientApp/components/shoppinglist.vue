<template>
    <div class="container">
        <div class="row">
            <div class="col-sm-12">
                <h2>Boodschappenlijst</h2>
                <ul class="list-group" v-if="list && list.length>0">
                    <li v-for="item in list" class="list-group-item list-group-item-action" v-on:click="setItemBought(item)">
                        <div class="form-check"> 
                            <input class="form-check-input" type="checkbox" :checked="item.bought" />
                                 {{item.amount}} {{item.ingredient.name}}
                          </div> 
                    </li>
                </ul>
                <div  v-if="list && list.length==0" class="alert alert-danger" role="alert">
                    Er zijn nog geen maaltijden in deze week dus ook geen boodschappen
                </div>
            </div>
            <div class="col-sm-12">

                <a :href="'/weekplanning/'+year+'/'+week" class="btn btn-secondary mt-3">Terug naar weekplan</a>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        data() {
            return {

                list: null
            }
        },
        props: [
            "week",
            "year"
        ],
        methods: {

            setItemBought: async function (item) {
                if (item.bought == null) {
                    item.bought = new Date();
                }
                else {
                    item.bought = null;
                }
                await this.$http.post('/api/ShoppingList/Save', item);

            }
        },

        async created() {

            try {
                let response = await this.$http.get('/api/ShoppingList/Get/' + this.year + "/" + this.week)

                this.list = response.data;
            } catch (error) {
                console.log(error)
            }

        }
    }
</script>
<style>
    .list-group-item-action {
        cursor: pointer;

    }
</style>
