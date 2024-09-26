<template>
  <div :class="{'i-text--required': isRequired, 'i-text': !isRequired}">
    <div class="d-flex" v-if="label"><label class="i-text__label" :for="id">{{ label }}</label></div>
    <input type="password" :class="style_class" :id="id" v-model="curValue" :readonly="isReadOnly" @focus="onFocus" @blur="onUnfocus">
  </div>
</template>

<script>
  export default {
    name: 'password-input',
    emits: ['update:modelValue'],
    props: {
      modelValue: String,
      showFiller: {
        type: Boolean,
        default: false
      },
      label: String,
      size: String,
      isReadOnly: {
          type: Boolean,
          default: false
      },
      isRequired: {
          type: Boolean,
          default: false
      }
    },
    methods: {
      onFocus() {
        this.vShowFiller = false
      },
      onUnfocus() {
        if(this.showFiller && !this.modelValue) this.vShowFiller = true
      }
    },
    data() {
      return {
        id: null,
        vShowFiller: this.showFiller
      }
    },
    computed: {
      curValue: {
        get: function () {
          // Display password circles when showFiller is enabled and           
          // password is empty
          if (!this.modelValue && this.vShowFiller) return 'ThisIsTrash'
          return this.modelValue
        },
        set: function (newValue) {
          // Remove circles after first user interaction
          if(!newValue) this.vShowFiller = false

          this.$emit('update:modelValue', newValue)
        }
      },
      style_class() {
        return 'i-text__field i-text input-' + (this.size || 'xxlarge')
      }
    },
    mounted() {
      var vm = this
      vm.id = 'input_t_' + vm._.uid
    },
    watch: {
      showFiller(newVal) {
          this.vShowFiller = newVal
      }
    }
  };
</script>