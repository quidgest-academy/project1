<template>
  <div id="maintenance_backup_container">
    <row v-if="!isEmptyObject(Model.ResultMsg)">
      <div class="alert alert-info">
        <span><b class="status-message">{{ Model.ResultMsg }}</b></span>
      </div>
      <br />
    </row>

    <row>
      <text-input v-model="Model.DbUser" :label="Resources.NOME_DE_UTILIZADOR58858" :isRequired="true"></text-input>
    </row>
    <row>
      <password-input v-model="Model.DbPsw" :label="Resources.PALAVRA_PASSE44126" :isRequired="true"></password-input>
    </row>
    <hr />

    <row>
      <h4>{{ Resources.BACKUP51008 }}</h4>
      <q-button
        b-style="primary"
        :label="Resources.BACKUP51008"
        @click="startBackup" />
    </row>
    <hr />
    <row>
      <qtable :rows="tBackups.rows"
              :columns="tBackups.columns"
              :config="tBackups.config"
              :totalRows="tBackups.total_rows">

            <template #actions="props">
              <q-button-group borderless>
                <q-button
                  :title="Resources.RESTAURAR57043"
                  @click="restoreBackupFile(props.row)">
                  <q-icon icon="circle-arrow-top" />
                </q-button>
                <q-button
                  :title="Resources.ELIMINAR21155"
                  @click="deleteBackupFile(props.row)">
                  <q-icon icon="remove" />
                </q-button>
              </q-button-group>
            </template>
            <template #Date="props">
                {{ formatDate(props.row.Date) }}
            </template>
            <template #Size="props">
                {{ props.row.Size }} Mb
            </template>
      </qtable>
    </row>
    <progress-bar :show="showPB" :withTitle="false" :text="Resources.BACKUP51008"></progress-bar>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import bootbox from 'bootbox';

  export default {
    name: 'maintenance_backup',
    mixins: [reusableMixin],
    data: function () {
      var vm = this;
      return {
        Model: {},
        showPB: false,
        tBackups: {
            rows: [],
            total_rows: 0,
            columns: [
            {
                label: () => vm.$t('ACOES22599'),
                name: "actions",
                slot_name: "actions",
                sort: false,
                column_classes: "thead-actions",
                row_text_alignment: 'text-center',
                column_text_alignment: 'text-center'
            },
            {
                label: () => vm.$t('DATA18071'),
                name: "Date",
                slot_name: 'Date',
                sort: true,
                initial_sort: true,
                initial_sort_order: "asc"
            },
            {
                label: 'Backup file',
                name: "Filename",
                sort: true
            },
            {
                label: () => vm.$t('TAMANHO__48454'),
                name: "Size",
                slot_name: 'Size',
                sort: true
            }],
            config: {
                table_title: () => vm.$t('RESTAURAR57043'),
                pagination_info: false
            }
        }
      };
    },
    methods: {
      fetchData: function () {
        var vm = this;
        QUtils.log("Fetch data - Maintenance - Backup");
        QUtils.FetchData(QUtils.apiActionURL('dbadmin', 'Backup')).done(function (data) {
          QUtils.log("Fetch data - OK (Maintenance - Backup)", data);
          $.each(data, function (propName, value) { vm.Model[propName] = value; });
          vm.setTableData();
        });
      },
      restoreBackupFile: function (bf) {
        var vm = this;
        console.log(bf);
        bootbox.confirm({
          title: vm.Resources.RESTAURAR_BACKUP53628,
          message: '<p><span class="e-icon glyphicons glyphicons-circle-arrow-top x2" width="32" height="32"></span> ' + vm.Resources.ESTA_OPERACAO_IRA_SU08117 + '</p>',
          buttons: {
            confirm: {
              label: vm.Resources.RESTAURAR57043,
              className: 'btn-primary'
            },
            cancel: {
              label: vm.Resources.CANCELAR49513,
              className: 'btn-secondary'
            }
          },
          callback: function (result) {
            if (result) {
              vm.Model.BackupItem = bf.Filename;
              vm.submitAction('Restore');
            }
          }
        });
      },
      deleteBackupFile: function (bf) {
        var vm = this;
        console.log(bf);
        bootbox.confirm({
          title: vm.Resources.APAGAR_BACKUP27193,
          message: '<p><span class="e-icon glyphicons glyphicons-remove x2" width="32" height="32"></span> Tem a acerteza que quer apagar?</p>',
          buttons: {
            confirm: {
              label: vm.Resources.APAGAR04097,
              className: 'btn-danger'
            },
            cancel: {
              label: vm.Resources.CANCELAR49513,
              className: 'btn-secondary'
            }
          },
          callback: function (result) {
            if (result) {
              vm.Model.BackupItem = bf.Filename;
              vm.submitAction('DeleteBackup');
            }
          }
        });
      },
      startBackup: function () {
        this.submitAction('Backup');
      },
      submitAction: function (action) {
        var vm = this;
        vm.showPB = true;
        QUtils.log("Request", QUtils.apiActionURL('DbAdmin', action));
        QUtils.postData('DbAdmin', action, vm.Model, null, function (data) {
          QUtils.log("Response", data);
          $.each(data, function (propName, value) { vm.Model[propName] = value; });
          vm.showPB = false;
          vm.Model.BackupItem = null;
          vm.setTableData();
        });
      },
      setTableData: function () {
          this.tBackups.rows = this.Model.BackupFiles || [];
          this.tBackups.total_rows = this.tBackups.rows.length;
      }
    },
    created: function () {
      // Ler dados
      this.fetchData();
    },
    watch: {
      // call again the method if the route changes
      '$route': 'fetchData'
    }
  };
</script>
