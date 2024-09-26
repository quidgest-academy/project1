<template>
  <div id="maintenance_data_quality_inconsistent_relations_container">
    <row>
      <card>
        <template #header>
          {{ Resources[Model.IncoherenceTitle] }}
        </template>
        <template #body>
          <template v-if="Model.IncoherenceType == 'IncoherentRelation'">
            <row>
              <select-input v-model="Model.RelationMode" v-if="Model.SelectLists" :options="Model.SelectLists.RelationsMode" :label="Resources.TIPO_DE_PESQUISA13226"></select-input>
            </row>
            <row>
              <checkbox-input v-model="Model.NullsIsChecked" :label="Resources.CONSIDERAR_AS_CHAVES25518"></checkbox-input>
            </row>
          </template>
          <row>
            <checkbox-input v-model="Model.ViewsIsChecked" :label="Resources.CONSIDERAR_AS_VIEWS07942"></checkbox-input>
          </row>
          <row>
            <label class="i-text__label">{{ Resources.ULTIMA_VERIFICACAO35305 }}</label>
            {{ formatDate(Model.LastUpdate) }}
          </row>
          <row>
            <q-button
              b-style="primary"
              :label="Resources.ATUALIZAR22496"
              @click="DataQualityStart" />
          </row>
        </template>
      </card>
    </row>
    <row v-if="Model.Active">
      <label :for="'progressbarVw_' + Model.Num">{{ Resources.PROGRESSO52692 }}</label>
      <div class="progress" :id="'progressbarVw_' + Model.Num">
        <div class="progress-bar progress-bar-striped progress-bar-animated" :style="{ width: dataPB.progress + '%' }">
          {{ dataPB.progress }}%
        </div>
      </div>
      <div>{{ dataPB.message }}</div>
    </row>
    <row v-if="!isEmptyObject(Model.IncoherentRelations)">
      <card>
        <template #header>
          {{ Resources.TIPO_DE_PESQUISA13226 }}: {{ Model.IncoherentRelations.length }} {{ Resources.CASOS25883 }} / {{ sumIR }} {{ Resources.TOTAL49307 }}
        </template>
        <template #body>
          <row>
              <qtable :rows="tIncoherentRelations.rows"
                      :columns="tIncoherentRelations.columns"
                      :config="tIncoherentRelations.config"
                      :totalRows="tIncoherentRelations.total_rows">
              </qtable>
          </row>
        </template>
      </card>
    </row>
    <row v-else-if="!isEmptyObject(Model.OrphanRelations)">
      <card>
        <template #header>
          {{ Resources.REGISTOS_ORFAOS43228 }}: {{ Model.OrphanRelations.length }} {{ Resources.CASOS25883 }} / {{ sumOR }} {{ Resources.TOTAL49307 }}
        </template>
        <template #body>
          <row>
              <qtable :rows="tOrphanRelations.rows"
                      :columns="tOrphanRelations.columns"
                      :config="tOrphanRelations.config"
                      :totalRows="tOrphanRelations.total_rows">
              </qtable>
          </row>
        </template>
      </card>
    </row>

  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';

  export default {
    name: 'maintenance_data_quality_inconsistent_relations',
    mixins: [reusableMixin],
    emits: ['updateData'],
    props: {
      Model: {
        required: true
      }
    },
    data: function () {
      var vm = this;
      return {
        dataPB: {
          progress: 0,
          message: ''
        },
        tIncoherentRelations: {
            rows: [],
            total_rows: 0,
            columns: [
                {
                    label: 'Table 1',
                    name: "Table1",
                    sort: true,
                    initial_sort: true,
                    initial_sort_order: "asc"
                },
                {
                    label: 'Foreign key 1',
                    name: "Fk1",
                    sort: true
                },
                {
                    label: 'Table 2',
                    name: "Table2",
                    sort: true
                },
                {
                    label: 'Foreign key 2',
                    name: "Fk2",
                    sort: true
                },
                {
                    label: 'Destination',
                    name: "Destination",
                    sort: true
                },
                {
                    label: 'Inconsistent Relations',
                    name: "CountIR",
                    sort: true
                },
                {
                    label: 'Path',
                    name: "Path",
                    sort: true
                },
                {
                    label: 'Query',
                    name: "Sql",
                    sort: true
                }
            ],
            config: {
            }
        },
        tOrphanRelations: {
            rows: [],
            total_rows: 0,
            columns: [
                {
                    label: 'Table 1',
                    name: "Table1",
                    sort: true,
                    initial_sort: true,
                    initial_sort_order: "asc"
                },
                {
                    label: 'Foreign key 1',
                    name: "Fk1",
                    sort: true
                },
                {
                    label: 'Destination',
                    name: "Destination",
                    sort: true
                },
                {
                    label: 'Orphan Records',
                    name: "CountOrphans",
                    sort: true
                },
                {
                    label: 'Query',
                    name: "Sql",
                    sort: true
                }
            ],
            config: {
            }
        }
      };
    },
    computed: {
      sumIR: function () {
        var vm = this, s = 0;
        if ($.isEmptyObject(vm.Model) || $.isEmptyObject(vm.Model.IncoherentRelations)) { return 0; }
        $.each(vm.Model.IncoherentRelations, (i, x) => { s += x.CountIR; });
        return s;
      },
      sumOR: function () {
        var vm = this, s = 0;
        if ($.isEmptyObject(vm.Model) || $.isEmptyObject(vm.Model.OrphanRelations)) { return 0; }
        $.each(vm.Model.OrphanRelations, (i, x) => { s += x.CountOrphans; });
        return s;
      }
    },
    methods: {
      DataQualityStart: function () {
        var vm = this;
        QUtils.postData('DbAdmin', 'DataQualityStart', vm.Model, null, function (data) {
          vm.startMonitorReindexProgress();
          vm.$emit('updateData', data);
        });
      },
      startMonitorReindexProgress: function() {
        if (this.Model.Active && this.dataPB.progress === 0) {
            setTimeout(this.checkProgress, 250);
        }
      },
      checkProgress: function () {
        var vm = this;
        QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'DataQualityProgress', { num: vm.Model.Num })).done(function (data) {
          vm.Model.Active = data.isActive;
          if (data.isActive) {
            vm.dataPB.progress = data.Count;
            vm.dataPB.message = data.Message;
            setTimeout(vm.checkProgress, 500);
          }
          else {
            vm.dataPB = {
              progress: 0,
              message: ''
            };
            vm.$emit('updateData');
          }
        });
      },
      fillTIncoherentRelations: function() {
          this.tIncoherentRelations.rows = this.Model.IncoherentRelations || [];
          this.tIncoherentRelations.total_rows = (this.Model.IncoherentRelations || []).length;
      },
      fillTOrphanRelations: function() {
          this.tOrphanRelations.rows = this.Model.OrphanRelations || [];
          this.tOrphanRelations.total_rows = (this.Model.OrphanRelations || []).length;
      }
    },
    created: function () {
      this.fillTIncoherentRelations();
      this.fillTOrphanRelations();
    },
    watch: {
        'Model.Active': {
            handler(newValue, oldValue) {
                if (newValue && !oldValue) {
                    this.startMonitorReindexProgress();
                }
            },
            deep: true
        },
        'Model.IncoherentRelations': {
            handler() {
                this.fillTIncoherentRelations();
            },
            deep: true
        },
        'Model.OrphanRelations': {
            handler() {
                this.fillTOrphanRelations();
            },
            deep: true
        }
    }
  };
</script>
