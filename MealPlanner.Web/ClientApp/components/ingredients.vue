<template>
        <div class="container">
  <div class="row">
    <div class="col-sm">     
        <h1>Boodschappen</h1>

        <p>Boodschappen/ingrediënten in ons systeem. Voeg er toe of verwijder foutieve, klik dan een bestaande aan en pas hem aan of verwijder</p>
        <form v-on:submit.prevent="saveIngredient">
            <label for="ingredientName">
              Naam:
            </label> 
            <input type="text" name="ingredientName" id="ingredientName" v-model="editItem.name" placeholder="zonder meervoud" autocomplete="off" />
            <input type="button" value="Verwijder" v-if="editItem.id" v-on:click="deleteIngredient()" />
            <input type="submit" value="Opslaan" />
            <input type="button" value="x" v-if="editItem.id" v-on:click="cancel()" />
        </form>  

        <p v-if="!ingredients"><em>Laden...</em></p>

        <ul class="tagCollection" v-if="ingredients">
            <li v-for="item in ingredients" v-on:click="edit(item)" v-bind:key="item.id">
                    {{ item.name }} 
             </li> 
        </ul> 
    </div>
  </div>
</div>
</template>

<script>
export default {
    data() {
        return {
            editItem:  {
                name: "",
                id: null
            },
            ingredients: null
        }
    },

    methods: {
        cancel: function()
        {
            this.editItem = {
                name: "",
                id: null
            }; 
        },
        edit: function(item)
        {  
            this.editItem = item;
        },
        saveIngredient: async function(event)
        {
            if (this.editItem.name != "")
            {  
                let response = await this.$http.post('/api/Ingredients/Save', this.editItem);
                if (response.data != "nope")
                {
                    if (response.data.id != this.editItem.id )
                    { 
                        this.editItem.id = response.data.id;
                        this.ingredients.push(this.editItem);
                    }
                }
                this.cancel();
            }
        },
        deleteIngredient: async function(event)
        {
            let response = await this.$http.post('/api/Ingredients/Delete', this.editItem);
             
            if (response.data == "done")
            { 
                this.ingredients.splice(this.ingredients.indexOf(this.editItem),1);
            }
            this.cancel();
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
