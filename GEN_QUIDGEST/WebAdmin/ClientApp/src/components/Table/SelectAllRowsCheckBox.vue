﻿<template>
    <th class="text-center justify-content-center" @click="$emit('select-all-row-checkbox')">
        <div class="custom-control custom-checkbox">
            <input type="checkbox" :indeterminate.prop="showIndeterminateState" class="custom-control-input vbt-checkbox" :checked="allRowsSelected" @change="$emit('update:allRowsSelected', $event.target.checked)" value=""/>
            <label class="custom-control-label"></label>
        </div>
    </th>
</template>

<script>
    export default {
        name: 'SelectAllRowsCheckBox',
        emits: ['select-all-row-checkbox', 'update:allRowsSelected'],
        props: {
            allRowsSelected: {
                type: Boolean,
                default: false
            },
            currentPageSelectionCount: {
                type: Number,
                default: 0
            },
        },
        computed: {
            showIndeterminateState() {
                return !this.allRowsSelected && this.currentPageSelectionCount > 0;
            },
        }
    }
</script>

<style scoped>
.custom-control-label {
  vertical-align: top;
}

</style>
