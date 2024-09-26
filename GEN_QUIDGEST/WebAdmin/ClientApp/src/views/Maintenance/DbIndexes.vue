<template>
  <div id="maintenance_dbindexes_container">
    <row>
      <card>
        <template #header>
          {{ Resources[Model.IndexTitle] }}
        </template>
        <template #body>
          <row>
            <label class="i-text__label">{{ Resources.ULTIMA_VERIFICACAO35305 }}</label>
            {{ formatDate(Model.LastUpdate) }}
          </row>
          <row>
            <q-button
              b-style="primary"
              :label="Resources.ATUALIZAR22496"
              @click="DBIndexesStart" />
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
    <row v-if="!isEmptyObject(Model.UnusedIndexesList)">
      <card>
        <template #header>
          {{ Resources[Model.IndexTitle] }}: {{ Model.UnusedIndexesList.length }} {{ Resources.CASOS25883 }}
        </template>
        <template #body>
          <row>
              <qtable :rows="tUnusedIndexes.rows"
                      :columns="tUnusedIndexes.columns"
                      :config="tUnusedIndexes.config"
                      :totalRows="tUnusedIndexes.total_rows">
              </qtable>
          </row>
        </template>
      </card>
    </row>
    <row v-else-if="!isEmptyObject(Model.RecommendedIndexesList)">
      <card>
        <template #header>
          {{ Resources[Model.IndexTitle] }}: {{ Model.RecommendedIndexesList.length }} {{ Resources.CASOS25883 }}
        </template>
        <template #body>
          <row>
              <qtable :rows="tRecommendedIndexes.rows"
                      :columns="tRecommendedIndexes.columns"
                      :config="tRecommendedIndexes.config"
                      :totalRows="tRecommendedIndexes.total_rows">
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
    name: 'maintenance_dbindexes',
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
            tUnusedIndexes: {
                rows: [],
                total_rows: 0,
                columns: [
                    {
                        label: () => vm.$t('ORDEM38897'),
                        name: "OrderCol",
                        sort: true,
                        initial_sort: true,
                        initial_sort_order: "asc"
                    },
                    {
                        label: () => vm.$t('TABELA44049'),
                        name: "ObjectName",
                        sort: true
                    },
                    {
                        label: () => vm.$t('INDICE13974'),
                        name: "IndexName",
                        sort: true
                    },
                    {
                        label: 'Seeks',
                        name: "UserSeeks",
                        sort: true
                    },
                    {
                        label: 'Scans',
                        name: "UserScans",
                        sort: true
                    },
                    {
                        label: 'Lookups',
                        name: "UserLookups",
                        sort: true
                    },
                    {
                        label: 'Updates',
                        name: "UserUpdates",
                        sort: true
                    },
                    {
                        label: () => vm.$t('REGISTOS53981'),
                        name: "TableRows",
                        sort: true
                    },
                    {
                        label: () => vm.$t('COLUNAS06085'),
                        name: "ColumnNames",
                        sort: true
                    },
                    {
                        label: () => vm.$t('QUERY_DE_ELIMINACAO04472'),
                        name: "Drop_Index",
                        sort: true
                    }
                ],
                config: {
                }
            },
            tRecommendedIndexes: {
                rows: [],
                total_rows: 0,
                columns: [
                    {
                        label: () => vm.$t('ORDEM38897'),
                        name: "OrderCol",
                        sort: true,
                        initial_sort: true,
                        initial_sort_order: "asc"
                    },
                    {
                        label: () => vm.$t('TABELA44049'),
                        name: "TableName",
                        sort: true
                    },
                    {
                        label: () => vm.$t('COLUNAS_COMPARADAS_P40915'),
                        name: "EqualityColumns",
                        sort: true
                    },
                    {
                        label: () => vm.$t('COLUNAS_COMPARADAS_P18476'),
                        name: "InequalityColumns",
                        sort: true
                    },
                    {
                        label: () => vm.$t('COLUNAS_INCLUIDAS_NA12269'),
                        name: "IncludedColumns",
                        sort: true
                    },
                    {
                        label: () => vm.$t('ULTIMO_SEEK53997'),
                        name: "Last_User_Seek",
                        sort: true
                    },
                    {
                        label: 'Seeks',
                        name: "UserSeeks",
                        sort: true
                    },
                    {
                        label: 'Scans',
                        name: "UserScans",
                        sort: true
                    },
                    {
                        label: () => vm.$t('MELHORIA____10458'),
                        name: "Avg_User_Impact",
                        sort: true
                    },
                    {
                        label: () => vm.$t('IMPACTO36269'),
                        name: "Avg_Estimated_Impact",
                        sort: true
                    },
                    {
                        label: () => vm.$t('QUERY_DE_CRIACAO34118'),
                        name: "Create_Statement",
                        sort: true
                    }
                ],
                config: {
                }
            }
        };
    },
    methods: {
      DBIndexesStart: function () {
        var vm = this;
        QUtils.postData('DbAdmin', 'DBIndexesStart', vm.Model, null, function (data) {
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
        QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'IndexesProgress', { num: vm.Model.Num })).done(function (data) {
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
      fillTUnusedIndexes: function() {
          this.tUnusedIndexes.rows = this.Model.UnusedIndexesList || [];
          this.tUnusedIndexes.total_rows = (this.Model.UnusedIndexesList || []).length;
      },
      fillTRecommendedIndexes: function() {
          this.tRecommendedIndexes.rows = this.Model.RecommendedIndexesList || [];
          this.tRecommendedIndexes.total_rows = (this.Model.RecommendedIndexesList || []).length;
      }
    },
    created: function () {
      this.fillTUnusedIndexes();
      this.fillTRecommendedIndexes();
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
        'Model.UnusedIndexes': {
            handler() {
                this.fillTUnusedIndexes();
            },
            deep: true
        },
        'Model.RecommendedIndexes': {
            handler() {
                this.fillTRecommendedIndexes();
            },
            deep: true
        }
    }
  };
</script>
