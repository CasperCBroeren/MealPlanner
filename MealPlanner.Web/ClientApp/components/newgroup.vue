<template>
    <div class="container">
        <div class="row" v-if="validateToken">
            <div class="col-sm-12 align-middle">

                <h1>Bijna klaar met je groep {{newGroupName}}</h1>
                <p>
                    Omdat we jou maaltijdplanner wel een beetje veilig willen maken, moet je via een one-time-password app de maaltijdplanner registreren.
                    Open daarom bijvoorbeeld Google Authenticator, One-password etc en scan de barcode. Deel de barcode met wie je de maaltijdplanner wil gebruiken.
                    Je kan de barcode later altijd in de opties van maaltijdplanner vinden.
                </p>
            </div>
            <div class="col-sm-4 align-middle">
                <img v-bind:src="qrCode" /> 
                    <div class="input-group input-group-lg">
                        <input type="password"
                               name="token"
                               maxlength="6"
                               class="form-control"
                               placeholder="One-time-password"
                               required autocomplete="off"
                               v-model="validationTotp"/>
                        <div class="input-group-append">
                            <button class="btn btn-primary" type="button" v-on:click="checkCode" id="button-addon2">Valideer</button>
                        </div>
                    </div>
                 
            </div>
             <div class="col-sm-12 align-middle mt-3" v-if="error">
                <div class="alert alert-danger" role="alert">
                    {{error}}
                </div>
            </div>
            
        </div>
        <div class="row" v-else>
            <div class="col-sm-12 align-middle">

                <h1>Welkom</h1>
                <p>
                    MaaltijdPlan kan voor je gezin/huishouden/groep mensen een uitkomst zijn om centraal maaltijden, weekplanning en boodschappenlijst af te stemmen.
                    Start een nieuwe groep of gebruik een bestaande
                </p>
            </div>
            <div class="col-sm-12 align-middle"> 
                    <div class="input-group input-group-lg">
                        <input type="text"
                               name="groupName"
                               maxlength="100"
                               class="form-control"
                               placeholder="Voer een naam in"
                               required autocomplete="off"
                               v-model="newGroupName"
                               />
                        <div class="input-group-append">
                            <button class="btn btn-primary" type="button" id="button-addon2" v-on:click="createGroup">Gaan</button>
                        </div>
                    </div>
                 
            </div> 
            <div class="col-sm-12 align-middle mt-3" v-if="error">
                <div class="alert alert-danger" role="alert">
                    {{error}}
                </div>
            </div>
            
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            error: null,
            validateToken: false,
            newGroupName: null,
            qrCode: null,
            validationTotp: null
        }
    },

    methods: {
        createGroup: async function () {
            try {
                let response = await this.$http.get('/api/group/create/' + this.newGroupName)
                if (response.data.qrCode != null) {
                    this.validateToken = true;
                    this.qrCode = response.data.qrCode;
                    this.newGroupName = response.data.name;
                    this.error = null;
                }
                else {
                    this.error = response.data;
                }
            } catch (error) {
                this.error = error;
            }
        },
        checkCode: async function () {
            try {
                let response = await this.$http.post('/api/group/Validate', {
                    token: this.validationTotp
                })
                if (response.data == "ok") {
                    this.$router.push("/");
                }
                else {
                    this.error = response.data;
                }
            } catch (error) {
                console.log(error)
            }
        }
    },

    async created() {
        try {
            let response = await this.$http.get('/api/group/getValidationToken' )
            if (response.data.qrCode !=null) {
                this.validateToken = true;
                this.qrCode = response.data.qrCode;
                this.newGroupName = response.data.name;
                this.error = null;
            } 
        } catch (error) {
            console.log(error);
        }
    }
}
</script>
