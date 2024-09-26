<template>
    <td class="text-center" v-on="!rowsSelectable ? { click: (event) => selectCheckbox(event)} : {}">
        <div class="custom-control custom-checkbox">
          <input type="checkbox" class="custom-control-input vbt-checkbox" v-model="checkboxSelected"/>
          <label class="custom-control-label"></label>
        </div>
    </td>
</template>


<script>
    export default {
        name: 'CheckBox',
        emits: ['add-row', 'remove-row'],
        props: {
            rowsSelectable: {
                type: Boolean,
                default: false
            },
            rowSelected: {
                type: Boolean,
                required: true
            }
        },
        data: function() {
            return {
                checkboxSelected:false
            }
        },
        methods: {
            selectCheckbox(event) {
                if (this.checkboxSelected) {
                    this.$emit('remove-row', event.shiftKey);
                } else {
                    this.$emit('add-row', event.shiftKey);
                }
            },
        },
        watch: {
            rowSelected(newVal) {
                this.checkboxSelected = newVal;
            }
        }
    }
</script>

<style scoped>
.custom-control-label {
  vertical-align: top;
}

</style>
