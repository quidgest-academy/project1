<template>
    <div id="message_queue_statistics_container">
        <row>
            <div class="control-join-group">
                <select-input v-model="queue" :options="queuesFilter" :label="Resources.QUEUE45251" :size="'xlarge'"></select-input>
                <datetime-picker v-model="Model.Stats.StartDate" :label="Resources.DATA_DE_INICIO37610"></datetime-picker>
                <datetime-picker v-model="Model.Stats.EndDate" :label="Resources.DATA_DE_FIM18270"></datetime-picker>
            </div>
        </row>
        <row>
            <q-button
                b-style="primary"
                :label="Resources.ATUALIZAR_ESTATISTIC07525"
                @click="UpdateStats" />
        </row>
        <row>
            <status v-if="!isEmptyObject(Status)" :Model="Status"></status>
        </row>
    </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import status from './Status.vue';

    export default {
        name: 'message_queue_statistics',
        mixins: [reusableMixin],
        components: { status },
        props: {
            Model: {
                required: true
            }
        },
        data: function () {
            //var vm = this;
            return {
                queue: '',
                queuesFilter: [
                    { Text: ' ', Value: '' }
                ],
                Status: { }
            };
        },
        computed: {
        },
        methods: {
            fillQueuesFilter: function() {
                var vm = this;
                $.each(vm.Model.MQueues.Queues, function(index, q) {
                    vm.queuesFilter.push({ Text: q.queue, Value: q.queue });
                });
            },
            UpdateStats: function () {
                var vm = this, assignedRoleId = [];
                $.each(vm.Model.MQueues.Acks, function (key, value) {
                    assignedRoleId.push(value.ackQueue);
                });

                QUtils.postData('MessageQueue', 'QueueProcessStats', {
                    queue: vm.queue,
                    dataINI: vm.Model.Stats.StartDate,
                    dataFIM: vm.Model.Stats.EndDate,
                    acks: assignedRoleId
                }, null, function (data) {
                    vm.Status = data;
                });
            }
        },
        created: function () {
            this.fillQueuesFilter();
        }
    };
</script>
