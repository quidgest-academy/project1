<template>  
    <div class="i-text">
        <div class="d-flex" v-if="label">
            <label class="i-text__label" :for="id">{{ label }}</label>
        </div>
        <input type="number" :class="style_class" :id="id" v-model="curValue" :readonly="isReadOnly">
    </div>
</template>

<script>
  export default {
    name: 'numeric-input',
    emits: ['update:modelValue'],
    props: {
      modelValue: [Number, String],
      label: String,
      size: String,
      isReadOnly: Boolean
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
      vm.id = "input_n_" + vm._.uid;

      //console.warn("Text-input Mounted");
    },
  };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
