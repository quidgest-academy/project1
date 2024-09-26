<template>
  <div id="maintenance_indexes_container">    
    <div class="alert alert-info">
      <label class="status-message" for="ResultMsg"><b>Indexes guidelines</b></label>
      <span style="white-space: pre-line">
        <br>
        SQL Server indexes are essentially copies of the data that already exist in the table, ordered and filtered in different ways to improve the performance of executed queries.<br>
        SQL Server indexes are an excellent tool for improving the performance of SELECT queries, but at the same time, SQL Server indexes have negative effects on data updates.<br>
        INSERT, UPDATE, and DELETE operations cause index updating (consuming time to do this) and therefore increasing the data that already exists in the table.<br>
        As a result, this increases the duration of transactions and the query execution and often can result in locking, blocking, deadlocking and quite frequent execution timeouts.<br>
        For large databases or tables, the storage space is also affected by redundant indexes.<br>
        A critical goal, of any SQL Server DBA, is to maintain indexes including creating required indexes but at the same time removing the ones that are not used.<br>
        <br>
        However, the data present contain only the data since the last SQL Server service restart.<br>
        Therefore, it is critical that there is a sufficient time since the last SQL Server restart that allows correctly determining which indexes are good candidates to be dropped or created.<br>
        <br>
        Seeks, scans and lookups operators are used to access SQL Server indexes.<br>
        <b class="status-info">Columns hints:</b><br />
        <b>Seeks</b> - Retrieve just selected rows.<br>
        <b>Scans</b> - Retrieve all the rows.<br>
        <b>Lookups</b> - Retrieve column information in the non-key data from the dataset of nonclustered indexes.<br>
        <b>Improvement (%)</b>  - Benefit that the queries (with columns presented in the index) could experience if the missing index was implemented (dropping query cost by this %).<br>
        <b>Impact</b>  - Improvement multiplied by Seeks and Scans. Meaning that a greater value will have more impact due to the fact that the missing index will be (statistically) used more often.
      </span>
    </div>

    <maintenance_dbindexes v-for="item in Model.Indexes" :key="item.IndexType" :Model="item" @updateData="updateData"></maintenance_dbindexes>

  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import maintenance_dbindexes from './DbIndexes.vue';

  export default {
    name: 'maintenance_indexes',
    mixins: [reusableMixin],
    components: { maintenance_dbindexes },
    data: function () {
      return {
        Model: {}
      };
    },
    methods: {
      fetchData: function () {
        var vm = this;
        QUtils.log("Fetch data - Maintenance - Indexes");
        QUtils.FetchData(QUtils.apiActionURL('dbadmin', 'Indexes')).done(function (data) {
          QUtils.log("Fetch data - OK (Maintenance - Indexes)", data);
          $.each(data, function (propName, value) { vm.Model[propName] = value; });
        });
      },
      updateData: function (data) {
        var vm = this;
        if($.isEmptyObject(data)) {
          vm.fetchData();
        }
        else {
          $.each(data, function (propName, value) { vm.Model[propName] = value; });
        }
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
