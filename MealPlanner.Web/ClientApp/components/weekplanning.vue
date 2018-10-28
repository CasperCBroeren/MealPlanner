<template>
    <div class="container">
        <div class="row">
            <div class="col-sm">
                <h1>Week planning</h1>
                <p>Plan een week met maaltijden en krijg een handige uitdraai van alle ingredienten</p>
                <form v-on:submit.prevent="saveIngredient">
                    <ul class="quickNav">
                        <li><a :href="'/weekplanning/' + prevYear + '/' + prevWeek">&lt;&lt;</a></li>
                        <li> Week {{week}} van {{year}}</li>
                        <li><a :href="'/weekplanning/' + nextYear + '/' + nextWeek">&gt;&gt;</a></li>
                    </ul>
                </form>
            </div>
        </div>
    </div>
</template>

<script>

    export default {
        data() {
            return {
                week: this.getWeek(new Date()),
                year: (new Date()).getFullYear(),
                forPersons: 2,
                meals: []
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
                    return this.week +1;
                }
            },
            nextWeek: function () {
                if (this.week == 51)
                    return 1;
                else {
                    return this.week +1;
                }
            }
        },
        methods: {
            getWeek: function (date) {
                var onejan = new Date(date.getFullYear(), 0, 1);
                var today = new Date(date.getFullYear(), date.getMonth(), date.getDate());
                var dayOfYear = ((today - onejan + 86400000) / 86400000);
                return Math.ceil(dayOfYear / 7)
            }
        },

        async created() {

            try {
                let response = await this.$http.get('/api/Ingredients/All')

                this.ingredients = response.data;
            } catch (error) {
                console.log(error)
            }

        }
    }
</script>
<style>
    .quickNav {
        width: 100%;
        list-style-type: none;

    }
    .quickNav li {

    }
</style>
