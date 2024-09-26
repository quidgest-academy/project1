<template>
  <div id="system_setup_identity_provider_container">
    <!-- Modal -->
    <div class="modal fade" id="system_setup_identity_provider" tabindex="-1" role="dialog" aria-labelledby="system_setup_identity_provider_Title" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="system_setup_identity_provider_Title">{{ Resources.FORNECEDOR_DE_IDENTI58587 }}</h5>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <row>
              <text-input v-model="Model.Name" :label="Resources.NOME47814" :isReadOnly="blockInputs" :size="'xlarge'" :isRequired="true"></text-input>
            </row>
            <row>
              <select-input v-model="Model.Type" :options="identityProviderSelect" :label="Resources.TIPO55111" :isReadOnly="blockInputs" :size="'xlarge'" :helpText="providerHelp"/>
            </row>
            <row v-for="c in tempConfig" :key="c.PropertyName">
              <text-input v-model="c.Value" :label="c.DisplayName" :isReadOnly="blockInputs" :isRequired="!c.Optional" :size="'xlarge'" :helpText="c.Description"></text-input>
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
              @click="SaveIdentityProvider" />
            <q-button
              v-else
              b-style="primary"
              :label="Resources.GRAVAR45301"
              @click="SaveIdentityProvider" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin, ReadProviderConfig, WriteProviderConfig } from '@/mixins/mainMixin';
  import { QUtils } from '../../utils/mainUtils';

  export default {
    name: 'identity_provider_modal',
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
        tempConfig: []
      };
    },
    watch: {
        "Model": {
            handler: function () {
                //we want to react to any change to the model
                this.tempConfig = ReadProviderConfig(this.Model, this.SelectLists.IdentityProviderTypeList);
            },
            deep: true
        }
    },
    methods: {
      SaveIdentityProvider: function () {
          let vm = this;
          WriteProviderConfig(this.tempConfig, this.Model, this.SelectLists.IdentityProviderTypeList);
        QUtils.postData('Config', 'SaveIdentityProvider', vm.Model, { appId: vm.$store.state.currentApp }, function (data) {
          if (data.success) {
            var eventData = { mode: vm.Model.FormMode, identityProvider: null };
            switch (vm.Model.FormMode) {
              case 'new':
              case 'edit':
                eventData.identityProvider = data.identityProvider;
                break;
              case 'delete':
                eventData.identityProvider = vm.Model;
                break;
            }
            vm.$emit('callback', eventData);
            $('#system_setup_identity_provider').modal('hide');
          }
        });
      },
      close() {
        this.$emit('close')
      }
    },
    computed: {
      blockInputs: function () {
        return this.Model.FormMode === 'delete';
      },
      identityProviderSelect: function () {
        return this.SelectLists.IdentityProviderTypeList.map(x => ({
          Text: x.DisplayName,
          Value: x.TypeFullName
        }));
      },
      providerHelp: function () {
        let vm = this;
        let p = vm.SelectLists.IdentityProviderTypeList.find(x => x.TypeFullName == vm.Model.Type);
        if (p) p = p.Description;
        return p;
      }
    }
  };
</script>
