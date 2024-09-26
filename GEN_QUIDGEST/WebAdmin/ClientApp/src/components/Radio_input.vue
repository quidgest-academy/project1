<template>
    <div class="i-radio__control">
        <div class="d-flex" v-if="label"><label class="i-text__label i-text">{{ label }}</label></div>
        <div class="form-check-inline">
            <label class="i-radio i-radio__label i-radio--inline" v-for="option in options" :key="ctrlID + '_' + option.Value">
                {{ option.Text }}
                <input type="radio" 
                    v-model="curValue" 
                    :disabled="isReadOnly"
                    :name="'radio_btn_' + ctrlID"
                    :value="option.Value" />
                <span class="i-radio__field"></span>
            </label>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'radio-input',
        emits: ['update:modelValue'],
        props: {
            modelValue: Boolean,
            options: Array,
            label: String,
            isReadOnly: { type: Boolean, default: false },
        },
        data: function () {
            return {
                ctrlID: null
            }
        },
        computed: {
            curValue: {
                get: function () { return this.modelValue; },
                set: function (newValue) { this.$emit('update:modelValue', newValue); }
            }
        },
        mounted: function () {
            var vm = this;//, comp = $(vm.$el);
            vm.ctrlID = vm.id || vm._.uid;

            //console.warn("Text-input Mounted");
        }
    };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
