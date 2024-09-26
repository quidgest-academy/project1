<template>
  <div class="i-text i-static-text">
    <div class="d-flex" v-if="label"><label class="i-text__label" :for="id">{{ label }}</label></div>
    <b v-if="bold" :id="id">{{ modelValue }}</b>
    <span v-else :id="id">{{ modelValue }}</span>
  </div>
</template>

<script>
  export default {
    name: 'static-text',
    props: {
      modelValue: [String, Number, Object],
      label: String,
      bold: {
        type: Boolean,
        default: false
      }
    },
    data: function () {
      return {
        id: null
      }
    },
    mounted: function () {
      var vm = this;//, comp = $(vm.$el);
      vm.id = 'static_text_' + vm._.uid;

      //console.warn("Static text Mounted");
    },
  };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
