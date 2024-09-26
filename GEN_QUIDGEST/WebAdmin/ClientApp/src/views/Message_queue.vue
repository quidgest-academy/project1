<template>
  <div id="message_queue_container">
    <div class="q-stack--column">
			<h1 class="f-header__title">
			{{ Resources.MESSAGE_QUEUEING34227 }}
			</h1>
		</div>
    <hr>
    
    <div>
        <ul class="nav nav-tabs c-tab c-tab__divider" id="system_setup_tabs" role="tablist">
            <li class="nav-item c-tab__item">
                <a class="nav-link c-tab__item-header active" id="dash-tab" data-toggle="tab" data-target="#dash" role="tab" aria-controls="dash" aria-selected="true">{{ Resources.DASHBOARD51597 }}</a>
            </li>
            <li class="nav-item c-tab__item">
                <a class="nav-link c-tab__item-header" id="queue-tab" data-toggle="tab" data-target="#queue" role="tab" aria-controls="queue" aria-selected="false">{{ Resources.EXPORTACAO_DE_MQ29750 }}</a>
            </li>
            <li class="nav-item c-tab__item">
                <a class="nav-link c-tab__item-header" id="db-tab" data-toggle="tab" data-target="#db" role="tab" aria-controls="db" aria-selected="false">{{ Resources.MENSAGENS53948 }}</a>
            </li>
            <li class="nav-item c-tab__item">
                <a class="nav-link c-tab__item-header" id="hist-tab" data-toggle="tab" data-target="#hist" role="tab" aria-controls="hist" aria-selected="false">{{ Resources.HISTORICO16073 }}</a>
            </li>
            <li class="nav-item c-tab__item">
                <a class="nav-link c-tab__item-header" id="stat-tab" data-toggle="tab" data-target="#stat" role="tab" aria-controls="stat" aria-selected="false">{{ Resources.ESTATISTICAS03241 }}</a>
            </li>
        </ul>
        <div class="tab-content c-tab__item-container">
            <!--Dashboard-->
            <div class="tab-pane c-tab__item-content active" id="dash" ref="dash" role="tabpanel" aria-labelledby="dash-tab">
                <dashboard v-if="isActiveTab('dash')"></dashboard>
            </div>
            <!--Queue-->
            <div class="tab-pane c-tab__item-content" id="queue" ref="queue" role="tabpanel" aria-labelledby="queue-tab">
                <queues v-if="!isEmptyObject(Model) && isActiveTab('queue')" :Model="Model"></queues>
            </div>
            <!--DB-->
            <div class="tab-pane c-tab__item-content" id="db" ref="db" role="tabpanel" aria-labelledby="db-tab">
                <messages v-if="!isEmptyObject(Model) && isActiveTab('db')" :Model="Model"></messages>
            </div>
            <!--Hist-->
            <div class="tab-pane c-tab__item-content" id="hist" ref="hist" role="tabpanel" aria-labelledby="hist-tab">
                <history v-if="!isEmptyObject(Model) && isActiveTab('hist')" :Model="Model"></history>
            </div>
            <!--Stat-->
            <div class="tab-pane c-tab__item-content" id="stat" ref="stat" role="tabpanel" aria-labelledby="stat-tab">
                <statistics v-if="!isEmptyObject(Model) && isActiveTab('stat')" :Model="Model"></statistics>
            </div>
        </div>
    </div>


  </div>
</template>

<script>
    // @ is an alias to /src
    import { reusableMixin } from '@/mixins/mainMixin.js';
    import { QUtils } from '@/utils/mainUtils'
    import dashboard from './Message_queue/Dashboard.vue';
    import queues from './Message_queue/Queues.vue';
    import messages from './Message_queue/Messages.vue';
    import history from './Message_queue/History.vue';
    import statistics from './Message_queue/Statistics.vue';

  export default {
    name: 'message_queue',
    mixins: [reusableMixin],
    components: { dashboard, queues, messages, history, statistics },
    data: function () {
      return {
        Model: { },
        activeTab: 'dash'
      };
    },
    methods: {
        fetchData: function () {
            var vm = this;
            QUtils.log("Fetch data - Message Queue");
            QUtils.FetchData(QUtils.apiActionURL('MessageQueue', 'Index')).done(function (data) {
                QUtils.log("Fetch data - OK (Message Queue)", data);
                $.each(data, function (propName, value) { vm.Model[propName] = value; });
            });
        },
        isActiveTab: function (tabName) {
          return this.activeTab === tabName;
        }
    },
    mounted: function () {
      var vm = this;
      vm.observer = new MutationObserver(mutations => {
        for (const m of mutations) {
          const newValue = m.target.getAttribute(m.attributeName);
          vm.$nextTick(() => {
            if (~newValue.indexOf('active')) {
              vm.activeTab = m.target.id;
            }
          });
        }
      });

      $.each(vm.$refs, function (ref) {
        vm.observer.observe(vm.$refs[ref], {
          attributes: true,
          attributeFilter: ['class'],
        });
      });
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
