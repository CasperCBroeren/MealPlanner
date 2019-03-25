<template>

    <div class="container">
        <h2>Opties</h2>

        <p>Hier is het mogelijk diverse zaken te regelen voor je maaltijdplan groep </p>
        <div class="row">
            <div class="col-sm-12">

                <div class="form-group">
                    <label for="groupToken">
                        Scan deze code in Google Authenticator of andere one-time-password tool
                    </label><br />
                    <img v-bind:src="qrToken" />
                    
                </div>
                <div class="input-group input-group-lg col-sm-4">
                    <input type="password" v-model="validationTotp" maxlength="6" name="validation" id="validation" placeholder="Validatie code" class="form-control" />
                    <div class="input-group-append">
                        <button v-on:click="checkCode" class="btn btn-primary">Controleer</button>
                    </div>
                </div>
                <div v-if="validateResult" class="text-info">
                    {{validateResult}}
                </div>


            </div>
        </div>
        <div class="row mt-4">
            <div class="col-sm-12">
                <h3>Uitloggen</h3>
                Klik <a  href="/logout">hier</a> om uit te loggen
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
                token: null,
                validateResult: null
            }
        },
        methods: {
            checkCode: async function () {
                try {
                    let response = await this.$http.post('/api/Options/Validate', {
                        token: this.validationTotp
                    });

                    this.validateResult = response.data;
                } catch (error) {
                    console.log(error)
                }
            }
        },

        async created() {
             
            try {
                let response = await this.$http.get('/api/Options')
                this.token = response.data.token;
                this.qrToken = response.data.qrToken;
            } catch (error) {
                console.log(error)
            }


        }
    }
</script>
