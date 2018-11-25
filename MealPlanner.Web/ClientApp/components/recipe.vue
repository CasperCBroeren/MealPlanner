<template>
    <div class="container">
        <div class="row" v-if="meal">
            <div class="col-sm-12">
                <h2>{{meal.name}}</h2>
                <ul>
                    <li v-for="ingredient in meal.ingredients">{{ingredient.amount}} {{ingredient.name}}</li>
                </ul>
                <hr/>
                <p>
                    {{meal.description}}
                </p>
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

                meal: null
            }
        },
        props: [
            "id",
            "week",
            "year"
        ],
        methods: {
             
        },

        async created() {

            try {
                let response = await this.$http.get('/api/Meal/Get/' + this.id)

                this.meal = response.data;
            } catch (error) {
                console.log(error)
            }

        }
    }
</script> 
