<template>
    <div class="autocomplete input-group"> 
        <input type="text" v-model="search" autocomplete="off" class="form-control" ref="input" :placeholder="placeholder"
               v-on:keydown.down="onArrowDown"
               v-on:keydown.up="onArrowUp"
               v-on:keydown.enter="onEnter"
               v-on:input="onChange" />
        <div class="input-group-append">
            <button class="btn btn-outline-secondary far fa-plus-square" type="button" v-on:click="finalizeSearch()"></button>
        </div>
        <ul class="autocomplete-results" v-show="isOpen">
            <li class="loading" v-if="isLoading">
                Laden....
            </li>
            <li v-else v-for="(result, i) in results"
                :key="i"
                :class="{ 'is-active': i === arrowCounter }"
                @click="setResult(result)">
                {{ getItemValue(result) }}
            </li>
        </ul>
    </div>
</template>
<script>
    export default {
        name: 'autocomplete',
        props: {
            itemValueProperty: {
                type: String,
                required: false,
                default: "value"
            },
            isAsync: {
                type: Boolean,
                required: false,
                default: false
            }, 
            items: {
                type: Array,
                required: false,
                default: () => [],
            }, 
            placeholder: {
                type: String,
                required: false,
                default: "..."
            }

        }, 
        data() {
            return {
                search: '',
                results: [],
                isOpen: false,
                isLoading: false,
                arrowCounter: 0, 
            };
        },
        methods: {
            getItemValue: function (item) { 
                return item[this.itemValueProperty];
            },
            handleClickOutside(evt) {
                if (!this.$el.contains(evt.target)) {
                    this.isOpen = false;
                    this.arrowCounter = -1;
                }
            },
            onArrowDown() {
                if (this.arrowCounter < this.results.length) {
                    this.arrowCounter = this.arrowCounter + 1;
                }
            },
            onArrowUp() {
                if (this.arrowCounter > 0) {
                    this.arrowCounter = this.arrowCounter - 1;
                }
            },
            onEnter() {
                if (this.isOpen && this.arrowCounter > -1) {
                    this.search = this.getItemValue(this.results[this.arrowCounter]);
                    this.isOpen = false;
                    this.arrowCounter = -1;
                }
                else {
                    this.finalizeSearch();
                }
            },
            finalizeSearch() { 
                this.$emit('keydown-enter', this.search);
                this.search = null;
                this.isOpen = false;
                this.arrowCounter = -1; 
            },
            setResult(item) {
                this.search = this.getItemValue(item);
                this.isOpen = false;
                this.arrowCounter = -1;
                this.$refs.input.focus();
            },
            onChange() {
                this.$emit('lookup', this.search); 
                if (this.isAsync) {
                    this.isLoading = true;
                }
                else {
                    this.isOpen = true;
                    this.filterResults();
                }
            },
            filterResults() {
                this.results = this.items.filter(item => item.toLowerCase().indexOf(this.search.toLowerCase()) > -1);
            }
        },
        watch: {
            items: function (value, oldValue) { 
                if (this.isAsync) { 
                    this.results = value; 
                    if (this.results.length > 0) {
                        this.isOpen = true;
                        this.isLoading = false;
                    }
                    else {
                        this.isOpen = false;
                        this.isLoading = true;
                    }
                    
                }
            }
        },
        mounted() {
            document.addEventListener('click', this.handleClickOutside)
        },
        destroyed() {
            document.removeEventListener('click', this.handleClickOutside)
        }

    };
</script>
<style>
    .autocomplete {
        position: relative;
    }

    .autocomplete-results {
        padding: 0;
        margin: 0;
        border: 1px solid #eeeeee;
        height: auto;
        position: absolute;
        width: 100%;
        background: #fff;
        z-index: 100;
        max-height: 200px;
        overflow-y: scroll;
        top: 40px;
    }

        .autocomplete-results li {
            list-style: none;
            text-align: left;
            padding: 4px 2px;
            cursor: pointer;
            border-top: 1px solid #ddd;
        }
        .autocomplete-results li:first-child {
            border: 0px;
        }
            .autocomplete-results li.is-active, .autocomplete-results li:hover {
                background-color: #ddd;
                color: #000;
            }
</style>
