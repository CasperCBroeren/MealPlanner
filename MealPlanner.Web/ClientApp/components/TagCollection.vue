<template>
    <ul class="tagCollection" v-if="items">
        <li v-for="(item, index) in items" v-bind:key="item[itemValueProperty]">
            {{ item[itemLabelProperty] }}
            <a v-on:click="removeItem(index, $event)"> x </a>
        </li>
    </ul>
</template>
<script>

    export default {
        name: "tagCollection",
        props: {
                itemValueProperty: {
                    type: String,
                    required: false,
                    default: "id"
                },
                itemLabelProperty: {
                    type: String,
                    required: false,
                    default: "value"
                },
                items: {
                    type: Array,
                    required: false,
                    default: () => [],
                },
                onItemRemoved:  {}
        },
        data() {
            return {
            }
        },
        methods: {
            filterValue: function (obj, key, value) {
                return obj.find(function (v) { return v[key] === value });
            },
            removeItem: function(i, event)
            {
                this.items.splice(i, 1);
                if (this.onItemRemoved != null)
                    this.onItemRemoved();
            },
            add: function(item)
            {
                if (this.filterValue(this.items, this.itemValueProperty, item[this.itemValueProperty]) == null) {
                    this.items.push(item);
                }
            }
        },

    }
</script>
<style>
  
</style>

