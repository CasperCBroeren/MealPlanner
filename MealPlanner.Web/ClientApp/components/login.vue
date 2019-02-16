<template>
    <div class="container">
        <div class="row">
            <div class="col-sm-4 pt-3 pr-5 ">
                <img src="images/icon_512.png" class="img-fluid"/>
            </div>
            <div class="col-sm-8 align-middle">

                <h1>Welkom</h1>
                <p>
                    Voer een groepsnaam  en je one-time-password van uit je app in. Ben je nieuw dan maak je eigen groep aan <a href="/new">hier</a>.
                </p>
                <h4>Inloggen</h4> 
                    <div class="input-group input-group-lg">
                        <input v-model="name" type="text" maxlength="100" name="name" id="name" placeholder="Groep" class="form-control" />
                        <input v-model="token" type="password" maxlength="6" name="token" id="token" placeholder="One-time-password" class="form-control" />
                        <div class="input-group-append">
                            <button type="button" class="btn btn-primary"v-on:click="login">Login</button>
                        </div>
                    </div>
                     
                    <div class="alert alert-danger mt-3" v-if="error">
                        {{error}}
                    </div> 
            </div>

        </div>
        <div class="row mt-5">
            <div class="col-sm-8 align-middle">
                <h4>Credits</h4>
                Idee: Casper &amp; Joyce<br />
                Uitvoering: Casper <br />
                Testen: Joyce <br />
                Libraries: Asp.net core 2, Joonsaw.SecurityHeaders, TwoFactorAuth.Net, Umi<br />
                Iconen door: <a href="https://www.flaticon.com/authors/monkik" title="monkik">monkik</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a> is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0" target="_blank">CC 3.0 BY</a>
            </div>
            <div class="col-sm-4">
                <h4>Feedback</h4>
                Stuur feedback naar c.broeren at gmail punt com
            </div>
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            error: null,
            name: null,
            token: null
        }
    },

    methods: {
        login: async function () {
            try {
                let response = await this.$http.post('/api/group/join/',
                    {
                        name: this.name,
                        token: this.token
                    })
                if (response.data.token) {
                    localStorage.setItem("jwtToken", response.data.token);
                    localStorage.setItem("groupName", response.data.name);
                    this.$router.push("/");
                }
                else {
                    this.error = response.data;
                }
            } catch (error) {
                this.error = error;
            }
        }
    },

    async created() {

        try {
            let response = await this.$http.get('/api/Login/State')

            this.ingredients = response.data;
        } catch (error) {
            console.log(error)
        }

    }
}
</script>
