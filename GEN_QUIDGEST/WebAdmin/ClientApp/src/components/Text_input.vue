<template>
  <div :class="{'i-text--required': isRequired, 'i-text': !isRequired}">
    <div class="d-flex" v-if="label">
      <label class="i-text__label" :for="id">{{ label }}</label>
      <span v-if="helpText" class="field-help glyphicons glyphicons-info-sign" :title="helpText"/>
    </div>
    <input type="text" :class="style_class" :id="id" v-model="curValue" :readonly="isReadOnly" :placeholder="placeholder">
  </div>
</template>

<script>
  export default {
    name: 'text-input',
    emits: ['update:modelValue'],
    props: {
      modelValue: String,
      label: String,
      size: String,
      isReadOnly: {
          type: Boolean,
          default: false
      },
      isRequired: {
          type: Boolean,
          default: false
      },
      placeholder: {
        type: String,
        default: null
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
        get: function () { return this.modelValue; },
        set: function (newValue) { this.$emit('update:modelValue', newValue); }
      },
      style_class: function () {
        return 'i-text__field i-text input-' + (this.size || 'xxlarge');
      }
    },
    mounted: function () {
      var vm = this;//, comp = $(vm.$el);
      vm.id = 'input_t_' + vm._.uid;

      //console.warn("Text-input Mounted");
    },
  };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
