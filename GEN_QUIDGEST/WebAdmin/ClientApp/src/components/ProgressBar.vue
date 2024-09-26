<template>
  <div class="c-modal hide" ref="modal" :id="id" data-backdrop="static" data-keyboard="false">
    <div class="c-modal__dialog modal-dialog-centered">
      <div class="c-modal__content">
        <div class="c-modal__header" v-if="withTitle">
          <h3 class="c-modal__header-title">{{ text }}</h3>
        </div>
        <div class="c-modal__body">
          <div class="progress" style="margin-bottom: 0px;">
            <div class="progress-bar progress-bar-striped progress-bar-animated" :style="{ width: progress + '%' }">
              <template v-if="!withTitle">{{ text }}</template>
            </div>
          </div>
        </div>

        <!-- Button -->
        <div v-if="withButton" class="justify-content-right">
          <button type="button" style="min-width: 20%;margin: 6px;" class="b-icon-text b-icon-text--primary float-right" @click="$emit('onButtonClick')">{{ buttonText }}</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'progress-bar',
    props: {
      show: {
        type: Boolean,
        default: false
      },
      progress: {
        type: Number,
        default: 100
      },
      text: {
        type: String,
        default: function () { return 'Processing...'; /*this.$t('A_PROCESSAR___18164');*/ }
      },
      withTitle: {
        type: Boolean,
        default: true
      },
      withButton: {
        type: Boolean,
        default: false
      },
      buttonText: {
        type: String,
        default: null
      }
    },
    data: function () {
      return {
        id: null
      }
    },
    methods: {
      evalShow: function () {
        var vm = this;
        if (vm.show) {
          $(vm.$refs.modal).modal({ show: true });
        } else {
          $(vm.$refs.modal).modal('hide');
        }
      }
    },
    mounted: function () {
      var vm = this;//, comp = $(vm.$el);
      vm.id = 'progress_bar_' + vm._.uid;
      vm.evalShow();
      //console.warn("progress bar Mounted");
    },
    watch: {
      'show': 'evalShow'
    },
    emits: [
      'onButtonClick'
    ]
  };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
