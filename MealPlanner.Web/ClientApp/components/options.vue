<template>

    <div class="container">
        <h2>Opties</h2>

        <p>Hier is het mogelijk diverse zaken te regelen voor je maaltijdplan groep </p>
        <div class="row">
            <div class="col-sm">
                 
                    <div class="form-group">
                        <label for="groupToken">
                            Scan deze code in Google Authenticator of andere TOTP tool
                        </label><br />
                        <img v-bind:src="qrToken" />
                    </div>
                    <div class="input-group input-group-lg col-4" >
                        <input type="text" v-model="validationTotp" maxlength="6" name="validation" id="validation" placeholder="Validatie code" class="form-control" />
                        <div class="input-group-append">
                            <button  v-on:click="checkCode" class="btn btn-primary">Controleer</button>
                        </div>
                    </div> 
                 


            </div>
        </div>
    </div>
</template>

<script>
    export default {
        data() {
            return {  
                qrToken: null,
                validationTotp: null,
            }
        },
        methods: {
            checkCode: async function () {
                try {
                    let response = await this.$http.get('/api/Options/Validate/' + this.validationTotp)

                    alert(response.data);
                } catch (error) {
                    console.log(error)
                }
            }
        },

        async created() {

            try {
                let response = await this.$http.get('/api/Options')
                 
                this.qrToken = response.data.qrToken;
            } catch (error) {
                console.log(error)
            }


        }
    }
</script>
