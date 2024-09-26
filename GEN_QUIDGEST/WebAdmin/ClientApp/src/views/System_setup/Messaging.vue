<template>
  <div id="system_setup_messaging_container">

    <row v-if="!isEmptyObject(resultMsg)">
      <div :class="['alert', statusError?'alert-danger':'alert-success']">
          <span>
              <b class="status-message">{{ resultMsg }}</b>
          </span>
      </div>
      <br />
    </row>

    <card>
      <template #header>
        Message broker
      </template>
      <template #body>
        <row>
          <checkbox-input v-model="Model.Enabled" :label="Resources.ATIVO_00196"></checkbox-input>
        </row>
        <row>
          <text-input v-model="Model.Host.Provider" label="Provider" :isReadOnly="true"></text-input>
        </row>
        <row>
          <text-input v-model="Model.Host.Endpoint" label="Endpoint" placeholder="amqp://localhost"></text-input>
        </row>
        <row>
          <text-input v-model="Model.Host.Username" :label="Resources.NOME_DE_UTILIZADOR58858"></text-input>
        </row>
        <row>
          <password-input v-model="Model.Host.Password" :label="Resources.PALAVRA_PASSE44126" :showFiller="true"></password-input>
        </row>
      </template>
    </card>
    <br />
    <card>
      <template #header>
        Publish
      </template>
      <template #body>
        <row v-for="pub in EnabledPublications">
           <checkbox-input :modelValue="pub.enabled" @update:modelValue="togglePub(pub, $event)" :label="pub.id"></checkbox-input>
           <span> - {{ pub.description }}</span>
        </row>
      </template>
    </card>
    <br />
    <card>
      <template #header>
        Subscribe
      </template>
      <template #body>
        <row v-for="sub in EnabledSubscriptions">
           <checkbox-input :modelValue="sub.enabled" @update:modelValue="toggleSub(sub, $event)" :label="sub.id"></checkbox-input>
           <span> - {{ sub.description }}</span>
        </row>
      </template>
    </card>

    <hr />
    <row>
      <q-button
        b-style="primary"
        :label="Resources.GRAVAR_CONFIGURACAO36308"
        @click="SaveConfigMessaging" />
    </row>

  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';

  export default {
    name: 'messaging',
    mixins: [reusableMixin],
    props: {
      Model: {
        required: true
      },
      Metadata: {
        required: true
      }
    },
    emits: ['updateModal'],
    data: function () {
      return {
        resultMsg: "",
        statusError: false
      };
    },
    computed: {
      EnabledPublications: function() {
        let vm = this;
        return this.Metadata.Publishers.map(p => { 
          return {
            id: p.Id,
            description: p.Description,
            enabled: vm.Model.EnabledPublications.indexOf(p.Id) != -1
          }
        });
      },
      EnabledSubscriptions: function() {
        let vm = this;
        return this.Metadata.Subscribers.map(p => { 
          return {
            id: p.Id,
            description: p.Description,
            enabled: vm.Model.EnabledSubscriptions.indexOf(p.Id) != -1
          }
        });
      }

    },
    methods: {

      togglePub(pub, checked) {
        this.makeSetHave(this.Model.EnabledPublications, pub.id, checked);
      },
      toggleSub(sub, checked) {
        this.makeSetHave(this.Model.EnabledSubscriptions, sub.id, checked);
      },

      makeSetHave(set, value, have) {
        let ix = set.indexOf(value);
        if(!have) { //set should not have the item
          if(ix != -1) {
            set[ix] = set[set.length-1];
            set.pop(); //remove the item
          }
        }
        else { //set should have the item
          if(ix == -1) {
            set.push(value); //add the item
          }
        }
      },


      SaveConfigMessaging: function () {
        var vm = this;
        QUtils.log("SaveConfigMessaging - Request", QUtils.apiActionURL('Config', 'SaveConfigMessaging'));
        QUtils.postData('Config', 'SaveConfigMessaging', vm.Model, null, function (data) {
          QUtils.log("SaveConfigMessaging - Response", data);          
          vm.$emit('updateModal', data);
          if (data.Success) {
              vm.resultMsg = vm.Resources.ALTERACOES_EFETUADAS10166;
              vm.statusError = false;
          } else {
              vm.resultMsg = data.Message;
              vm.statusError = true;
          }

        });
      }
    }
  };
</script>
