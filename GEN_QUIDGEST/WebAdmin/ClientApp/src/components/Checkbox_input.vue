<template>
  <div class="i-checkbox" :class="{ 'i-checkbox--disabled': isReadOnly }">
    <label class="i-checkbox i-checkbox__label" :for="ctrlID">
      {{ label }}
      <input type="checkbox" v-model="curValue" :id="ctrlID" :data-id="dataId" :disabled="isReadOnly" v-on:click="_click" class="i-checkbox__field i-checkbox input-small">
      <span class="i-checkbox__field"></span>
    </label>
  </div>
</template>

<script>
  export default {
    name: 'checkbox-input',
    emits: ['update:modelValue', 'click'],
    props: {
      modelValue: Boolean,
      label: String,
      size: String,
      isReadOnly: { type: Boolean, default: false },
      clickData: Object,
      id: String,
      dataId: { type: String, default: '' }
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
    methods: {
      _click: function () {
        this.$emit('click', this.clickData || {});
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
