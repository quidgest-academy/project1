<template>
  <div id="system_setup_more_property_container">
    <!-- Modal -->
    <div class="modal fade" id="system_setup_more_property" ref="system_setup_more_property" tabindex="-1" role="dialog" aria-labelledby="system_setup_more_property_Title" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="system_setup_more_property_Title">{{ Resources.PROPERTY43977 }}</h5>
            <button type="button" class="close" aria-label="Close" @click="close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
              <div class="alert c-alert c-alert--warning fade show" role="alert" v-if="!isEmptyObject(erros)">
                  <i class="glyphicons glyphicons-alert c-alert__icon"></i>
                  <div class="c-alert__text">
                      {{erros}}
                  </div>
              </div>
              <row v-if="hasInitProperties && !showNewKeyInput && !blockInputKey">
                  <select-input v-model="Model.Key" v-if="SelectLists" :options="SelectLists.PropertyList" :label="Resources.KEY01046" :is-read-only="blockInputKey" :size="'xlarge'"></select-input>
                  <button type="button" class="b-icon--secondary i-input-group__button--secondary" @click="showNewKeyInput=true" :title="Resources.INSERT_NEW_KEY15186">
                    <slot name="new-button-text">
                      <i class="glyphicons glyphicons-pencil i-input-group__tag-icon i-input-group__button-icon"></i>
                    </slot>
                  </button>
              </row>
              <row v-else>
                  <text-input v-model="Model.Key" :label="Resources.KEY01046" :is-read-only="blockInputKey" :size="'xlarge'"></text-input>
                  <button type="button" class="b-icon--secondary i-input-group__button--secondary" @click="showNewKeyInput=false" :title="Resources.LIST_DEFAULT_KEYS58194" v-show="hasInitProperties && !blockInputKey">
                    <slot name="list-button-text">
                      <i class="glyphicons glyphicons-list i-input-group__tag-icon i-input-group__button-icon"></i>
                    </slot>
                  </button>
              </row>
              <row>
                  <text-input v-model="Model.Val" :label="Resources.VALUE10285" :is-read-only="blockInputValue" :size="'xlarge'"></text-input>
              </row>
          </div>
          <div class="modal-footer">
            <q-button
              :label="Resources.CANCELAR49513"
              @click="close" />
            <q-button
              v-if="Model.FormMode === 'delete'"
              b-style="danger"
              :label="Resources.APAGAR04097"
              @click="SaveMoreProperty" />
            <q-button
              v-else
              b-style="primary"
              :label="Resources.GRAVAR45301"
              @click="SaveMoreProperty" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '../../utils/mainUtils';
  import bootbox from 'bootbox';

  export default {
    name: 'more_property_modal',
    emits: ['callback', 'close'],
    props: {
      Model: {
        required: true
      },
      SelectLists: {
        required: true
      }
    },
    mixins: [reusableMixin],
    data: function () {
      return {
        erros: '',
        showNewKeyInput: false
      };
    },
    methods: {
      SaveMoreProperty: function () {
        var vm = this;
        vm.erros = '';
        QUtils.postData('Config', 'SaveMoreProperty', vm.Model, { appId: vm.$store.state.currentApp }, function (data) {
          if (data.emptyKey) { vm.erros = vm.Resources.KEY_CANNOT_BE_EMPTY_64382; }
          else if (data.emptyVal) { vm.erros = vm.Resources.VALUE_CANNOT_BE_EMPT24668; }
          else if (!data.success) { vm.erros = vm.Resources.THIS_KEY_ALREADY_EXI09944; }
          else {
            var eventData = { mode: vm.Model.FormMode, moreProperty: null };
            switch (vm.Model.FormMode) {
              case 'new':
              case 'edit':
                eventData.moreProperty = data.moreProperty;
                break;
              case 'delete':
                if (data.initProp) {
                  eventData.moreProperty = data.moreProperty;
                  bootbox.alert(vm.Resources.CANNOT_DELETE_THIS_P45050);
                } else {
                  eventData.moreProperty = vm.Model;
                }
                break;
            }
            $(vm.$refs.system_setup_more_property).modal('hide');
            vm.$emit('callback', eventData);
          }
        });
      },
      close: function () {
        $(this.$refs.system_setup_more_property).modal('hide');
        this.$emit('close');
      }
    },
    computed: {
      blockInputKey: function () {
        return this.Model.FormMode === 'delete' || this.Model.FormMode === 'edit';
      },
      blockInputValue: function () {
        return this.Model.FormMode === 'delete';
      },
      hasInitProperties: function () {
        return this.SelectLists.PropertyList.length > 0;
      }
    },
    beforeCreate: function () {

    },
    mounted: function () {
        var vm = this;
        $(vm.$refs.system_setup_more_property).modal('show');
        $(vm.$refs.system_setup_more_property).on('hidden.bs.modal', function () {
            vm.$emit('close');
        });
    },
    beforeMount: function () {
      //var vm = this;
    },
    created: function () {
      // Ler dados + Resources
      //var vm = this;
      //.each(vm, function (propName, value) { vm.Model[propName] = value; });
      //console.log(vm.Model);
    },
    watch: {
      // call again the method if the route changes
      //'$route': 'fetchData'
    }
  };
</script>
