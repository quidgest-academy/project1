<template>
    <div class="i-dbedit">
        <div class="d-flex">
            <label :for="id" v-if="label" class="i-text__label">{{ label }}</label>
            <span v-if="helpText" class="field-help glyphicons glyphicons-info-sign" :title="helpText"/>
        </div>
        <div :class="style_class" :id="'select_input_container_' + id">
            <multiselect v-model="curValue" :options="options" :id="id" label="Text" :disabled="isReadOnly"></multiselect>
        </div>
    </div>
</template>

<script>
  // https://vue-multiselect.js.org
  import Multiselect from '@/components/multiselect/multiselect_main';
  //import Multiselect from 'vue-multiselect';

  export default {
    name: 'select-input',
    components: { 'multiselect': Multiselect },
    emits: ['update:modelValue'],
    props: {
      modelValue: [String, Number, Object],
      options:{
        type:Array,
        default(){
            return []
        }
      } ,
      label: String,
      size: String,
      isReadOnly: {
          type: Boolean,
          default: false
      },
      helpText: {
        type: String,
        default: null        
      }
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
              selOption = $.grep(vm.options, function (option) { return vm.compareValues(option.Value, vm.modelValue); }),
            Text = (selOption || []).length == 1 ? selOption[0].Text : '';
              return { Value: vm.modelValue, Text };
        },
            set: function (newValue) { this.$emit('update:modelValue', $.isEmptyObject(newValue) ? newValue : newValue.Value); }
      },
      style_class: function () {
        return 'input-' +(this.size || 'xxlarge');
      }
    },
    mounted: function () {
      var vm = this;//, comp = $(vm.$el);
      vm.id = vm._.uid;

      //console.warn("Select-input Mounted");
    },
    methods: {
      customLabel(obj) {
        return `${obj.Text}`
      },
     /**
     * Case insensitve comparison of two values
     * @param  {Object||String||Integer||Number} fValue passed first value
     * @param  {Object||String||Integer||Number} sValue passed second value
     * @returns {Boolean} returns true if values are equals
     */
    compareValues(fValue, sValue) {
              var _fValue = fValue, _sValue = sValue;
          
          if (typeof fValue === 'string') {
              _fValue = fValue.toLowerCase();
          }
          if (typeof sValue === 'string') {
              _sValue = sValue.toLowerCase();
          }
          return _fValue == _sValue;
      }
    }
  };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<!--<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>-->
<style scoped lang="scss">
  @import '../assets/styles/plugins/vue-multiselect.min.css';
</style>
