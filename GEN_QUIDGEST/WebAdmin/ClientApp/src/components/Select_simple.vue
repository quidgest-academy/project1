<template>
    <div class="dropdown i-select select-simple" :class="{ 'dropleft': side == 'left' }">
        <div class="dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" :id="'dropdown_' + id" ref="toggle">
            <i :class="style_class" v-if="!isEmptyObject(icon)" style="margin-right:0.25rem; margin-right:0.25rem;"></i>
            <span v-if="showValue">{{ curValue.Text }}</span>
            <span v-else-if="!isEmptyObject(staticText)">{{ staticText }}</span>
        </div>
        <div ref="container" 
             class="dropdown-menu"
             :aria-labelledby="'dropdown_' + id">
            <template v-for="option in options" :key="option.Value">
                <a class="dropdown-item" 
                   :class="{ 'active' : option.Value == modelValue }" 
                   @click.stop="selectValue(option)">
                    <slot name="option" :option="option">{{ option.Text }}</slot>
                </a>
            </template>
        </div>
    </div>
</template>

<script>
    import { reusableMixin } from '@/mixins/mainMixin';
    export default {
        name: 'select_simple',
        emits: ['update:modelValue'],
        mixins: [reusableMixin],
        props: {
            modelValue: [Object, String, Number],
            options: Array,
            label: String,
            icon: String,
            side: {
                type: String,
                default: 'right'
            },
            showValue: {
                type: Boolean,
                default: true
            },
            staticText: String
        },
        data: function () {
            return {
                id: null
            }
        },
        computed: {
            curValue: {
                get: function () {
                    var vm = this,
                        selOption = $.grep(vm.options, function (option) { return option.Value == vm.modelValue; }),
                        Text = (selOption || []).length == 1 ? selOption[0].Text : '';
                    return { Value: vm.modelValue, Text };
                },
                set: function (newValue) { this.$emit('update:modelValue', $.isEmptyObject(newValue) ? newValue : newValue.Value); }
            },
            style_class: function () {
                return 'glyphicons glyphicons-' + (this.icon || 'option-horizontal') + ' e-icon';
            }
        },
        methods: {
            selectValue: function (option) {
                this.$emit('update:modelValue', option.Value);
            }
        },
        mounted: function () {
            var vm = this;//, comp = $(vm.$el);
            vm.id = vm._.uid;

            $(vm.$refs.container).mouseleave(function () {
                $(vm.$refs.toggle).dropdown('toggle');
            });
            //console.warn("Select-simple Mounted");
        }
    };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
    .select-simple {
        background: transparent !important;
        border: transparent !important;
        cursor: pointer;
    }
</style>
