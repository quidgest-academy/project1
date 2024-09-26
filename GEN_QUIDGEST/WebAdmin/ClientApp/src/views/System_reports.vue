<template>
  <div id="system_reports_container">
    <div class="q-stack--column">
			<h1 class="f-header__title">
			{{ Resources.RELATORIO_DO_SISTEMA49744 }}
			</h1>
		</div>
    <hr>
    
    <row>
      <select class="form-control" style="float:right; width: 200px;" v-model="currentApp">
        <option v-for="app in Model.Applications" v-bind:key="app.Id" v-bind:value="app.Id">{{ app.Name }}</option>
      </select>
    </row>
    <row>
      <div v-if="!isEmptyObject(Model.ResultMsg)" class="alert alert-danger">
        <span>
          <b class="status-message" v-html="Model.ResultMsg"></b>
        </span>
      </div>
      <template v-else>
        <row>
            <div class="input-group i-input-group">
                <input type="text" v-model="searchError" ref="searchError" class="form-control i-input-group__field" placeholder="Search for..." v-on:keyup="searchErro">
                <div class="input-group-append i-input-group--right">
                    <button class="btn b-icon-text--primary i-input-group__button--primary" type="button" id="searchErrorBtn" ref="searchErrorBtn" @click.stop="highlightSearchErro">
                      <span class="glyphicons glyphicons-search i-input-group__tag-icon i-input-group__button-icon"></span>
                    </button>
                </div>
            </div>
        </row>

        <row>
          <div id="textAreaErrorLog" ref="textAreaErrorLog" class="i-textarea__field i-textarea"><span style="white-space: pre-line">{{ Model.ErrorLog }}</span></div>
        </row>
      </template>
    </row>

  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import 'jquery-highlight';

  export default {
    name: 'system_reports',
    mixins: [reusableMixin],
    data: function () {
      return {
        Model: {},
        searchError: ''
      };
    },
    methods: {
      fetchData: function () {
        var vm = this;
        QUtils.log("Fetch data - System reports");
        QUtils.FetchData(QUtils.apiActionURL('ErrorLog', 'Index')).done(function (data) {
          QUtils.log("Fetch data - OK (System reports)", data);
          $.each(data, function (propName, value) { vm.Model[propName] = value; });
          // Select the first exists application
          if ($.isEmptyObject(vm.currentApp) && !$.isEmptyObject(vm.Model.Applications)) {
            vm.currentApp = vm.Model.Applications[0].Id;
          }
        });
      },
      searchErro: function (event) {
        if (event.keyCode == 13) {
          this.highlightSearchErro();
        }
      },
      highlightSearchErro: function () {
          $(this.$refs.textAreaErrorLog).unhighlight();
          $(this.$refs.textAreaErrorLog).highlight(this.searchError);
      }
    },
    created: function () {
      // Ler dados
      this.fetchData();
    },
    watch: {
      // call again the method if the route changes
      '$route': 'fetchData',
      'currentApp': 'fetchData'
    }
  };
</script>
