<template>
    <div id="role_container">
        <card>
            <template #header>
              <h2>{{Model.ModuleDescription}} - <strong v-if="Model.Description">{{$t(Model.Designation)}}</strong></h2>
            </template>
            <template #body>
              <row id="row-description" v-if="Model.Description">
                <p id="title-parag">{{Resources.DESCRICAO07528}}:</p>
                  <p>{{$t(Model.Description)}}</p>
               </row>
               <row>
                    <card>
                        <template #header>
                          <h2>{{Resources.UTILIZADORES39761}}</h2>
                        </template>
                        <template #body>
                            <qtable
                                :rows="Model.UserAboveList"
                                :columns="userListColumns">
                                <template #role="props">
                                    <span class="role-tag">
                                        {{$t(props.row.Designation)}}
                                    </span>
                                </template>
                            </qtable>
                        </template>
                    </card>
                </row>
                <row>
                    <card>
                        <template #header>
                            {{Resources.HIERARQUIA22557}}
                        </template>
                        <template #body>
                            <div class="row">
                                <div class="col-sm">
                                    <svg class="graph" ref="svg">
                                      <g ref="graph"/>
                                    </svg>
                                </div>
                            </div>
                        </template>
                    </card>
                  <br>
                    <q-button
                        :label='Resources.VOLTAR01353'
                        @click="goBack" />
                </row>
            </template>
        </card>
    </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import { PathUtils } from '@/utils/PathFinder';
  import dagreD3 from "dagre-d3";
  import * as d3 from "d3";

  export default {
        name: 'role-view',
        mixins: [reusableMixin],
        mounted() {
            this.$nextTick(() =>
            {
                // access our input using template refs, then focus
                this.$refs.svg.focus()
                this.redrawGraph(); // <= It gives an error because the fetchRole may not have completed the load yet. The await in 'created' does not stop the process..
            })
        },
        created: function () {
            // Ler dados
            this.fetchRole();
        },
        data: function () {
            var vm = this;
            return {
                Model: {
                    Parents: [],
                    Children : [],
                    ModuleDescription: '',
                    Description: ''
                },
                graphic: [{
                    name: String,
                    imports: [],
                    class: String
                }],
                userListColumns: [
                    {
                        label: () =>  vm.$t('UTILIZADOR52387'),
                        name: "UserName",
                        sort: true,
                        initial_sort: true,
                        initial_sort_order: "asc"
                    },
                    {
                        label: () => vm.$t('ULTIMA_ALTERACAO22785'),
                        name: "ChangedDate",
                        sort: true,
                        initial_sort: false,
                    },
                    {
                        label: () => vm.$t('ROLE60946'),
                        name: "role",
                        slot_name : "role",
                        sort: false,
                    }
                ],
                graph: null
            };
        },
    methods: {
        fetchRole: async function () {
            var vm = this;
            QUtils.log("Fetch data - Role");
			let data = await QUtils.FetchData(QUtils.apiActionURL('Role', 'GetRole', { module: vm.$route.params.module, roleId: vm.$route.params.role }));
			QUtils.log("Fetch data - OK (Role)");
			vm.Model = data;
			vm.RoleOnly = 'only';
        },
        viewRole: async function (role) {
            //In vue, instead of reloading the page, you're supposed to reload the data
            this.$route.params.role = role;
            await this.fetchRole();
            this.resetGraph();
            this.redrawGraph();
        },
        assignRole: function () {
            this.$router.push({
                name: 'assign_role',
                params: { role: this.$route.params.role, module: this.$route.params.module, culture: this.currentLang, system: this.currentYear }
            });
        },
        goBack: function () {
            return this.$router.go(-1);
        },
        resetGraph: function () {
            while(this.graphic.length > 1){
                this.graphic.pop();
            }
        },
        redrawGraph: async function(){

            var svg = d3.select(this.$refs.svg);

            this.graph = new dagreD3.graphlib.Graph();

            this.graph.setGraph({});

            this.graph.graph().rankdir = "TB";
            this.graph.graph().ranker = "network-simplex"; //network-simplex, longest-path, tight-tree

            //graph represents the nodes of the graph
            //graphic represents the connections of the graph

            //give the right designation to each node
            this.Model.Children.forEach(child => {
                child.Designation = this.$t(child.Designation)
            });

            var vm = this;
            //Setup the label for the origin and its edges( imports )
            this.graph.setNode(this.$t(this.Model.Designation), {label: this.$t(this.Model.Designation), class: "origin"});
            vm.graphic.push({
                    name: vm.$t(vm.Model.Designation),
                    imports: vm.Model.Children,
                    class: "origin"
            });

            //Setup the children of the selected role and its edges
            //For each child we also fetch its children
            for(const element of this.Model.Children) {
                vm.graph.setNode(this.$t(element.Designation), {label: this.$t(element.Designation), class: "dests"});

                QUtils.log("Fetch data - Role");
				let data = await QUtils.FetchData(QUtils.apiActionURL('Role', 'GetRole', { module: element.Module, roleId: element.Role }));
                    QUtils.log("Fetch data - OK (Role)");
                    data.Children.forEach(child => {
                        child.Designation = vm.$t(child.Designation)
                    });

                    vm.graphic.push({
                        name: vm.$t(element.Designation),
                        imports: data.Children,
                        class: "dests"
                    });
            }

            //Setup the Parents of the selected role and its edges
            //For each parent we also fetch its children
            for(const element of this.Model.Parents) {
                vm.graph.setNode(this.$t(element.Designation), {label: this.$t(element.Designation), class: "sources"});

                QUtils.log("Fetch data - Role");
				let data = await QUtils.FetchData(QUtils.apiActionURL('Role', 'GetRole', { module: element.Module, roleId: element.Role }));
                    QUtils.log("Fetch data - OK (Role)");
                    data.Children.forEach(child => {
                        child.Designation = vm.$t(child.Designation)
                    });

                    vm.graphic.push({
                        name: vm.$t(element.Designation),
                        imports: data.Children,
                        class: "dests"
                    });
            }
            this.graphic.shift();


            // The calculated graphic is used to calculate the reachability matrix
            this.reach = PathUtils.reachabilityMatrix(this.graphic);
            this.updateEdges();

            // Set up zoom support

           this.zoom = d3.zoom().on("zoom", this.nodeZoomHandler);

            svg.call(this.zoom);
            svg.on("dblclick.zoom", null);

            // Create the renderer
            this.updateRender();

            //set up node click event
            svg.selectAll("g.node")
                .on("dblclick", this.nodeDblClickHandler);
                //.on("click", this.nodeClickHandler);
        },
        updateEdges: function() {
            //remove all the previous edges
            this.graph.edges().forEach( edge=> {
                this.graph.removeEdge(edge.v, edge.w);
            }, this);

            // setup the edges using the calculated graphic
            this.graphic.forEach( i=> {
                i.imports.forEach( src=> {
                    if(this.graph.node(this.$t(src.Designation)))
                    {
                        //the calculated reach is used to check if a edge is redundant
                        if(PathUtils.isPrimaryEdge(this.reach, i.name, this.$t(src.Designation)))
                        {
                            var clEdge = "";
                            var wgEdge = 1;
                            this.graph.setEdge(i.name, this.$t(src.Designation), {label:"", curve: d3.curveBasis, class: clEdge, weight: wgEdge });
                        }
                    }
                }, this);
            }, this);
        },
        updateRender: function() {

            var svg = d3.select(this.$refs.svg);
            this.inner = svg.select("g");
            var render = new dagreD3.render();
            // Run the renderer. This is what draws the final graph.

            render(this.inner, this.graph);

            //set scale
            this.fitSize();

        },
        fitSize: function() {
            var width = this.$refs.svg.clientWidth;

            // Center the graph
            //var initialScale = 0.75;
            //resize to fit
            var initialScale = (width * 1.0) / (this.graph.graph().width + 50.0);

            if(initialScale > 0.75)
                initialScale = 0.75;
            this.rescale(initialScale);
        },
        rescale: function(newScale) {
            var svg = d3.select(this.$refs.svg);

            var width = this.$refs.svg.clientWidth;

            // Center the graph
            this.zoom.transform(svg, d3.zoomIdentity
                .translate((width - this.graph.graph().width * newScale) / 2, 20)
                .scale(newScale)
                );

            //minimize the height of the container
            svg.attr('height', this.graph.graph().height + 40);

        },
        nodeDblClickHandler : function(d) {

            this.Model.Children.forEach(element => {
                if(this.$t(element.Designation) == d.path[1].__data__){
                    this.viewRole(element.Role)
                    return;
                }
            })

            this.Model.Parents.forEach(element => {
                if(this.$t(element.Designation) == d.path[1].__data__){
                    this.viewRole(element.Role)
                    return;
                }
            })
        },
        nodeZoomHandler : function(e) {
            this.$refs.graph.setAttribute("transform", e.transform);
        }
    }
  };
</script>

<style>
  h1 {
  font-size : 32px;
  }

  h2 {
  font-size: 24px;
  }

  h3 {
  font-size: 18px;
  }
  .graph .node rect,
  .graph .node circle,
  .graph .node ellipse {
  stroke: #333;
  fill: #fff;
  stroke-width: 1px;
  opacity: 1;
  transition: opacity 0.5s;
  }
  .graph text{
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
  font-family: sans-serif;
  opacity: 1;
  transition: opacity 0.5s;
  }
  .graph .edgePath path {
  stroke: #666;
  fill: #666;
  stroke-width: 1.5px;
  opacity: 1;
  transition: opacity 0.5s;
  }
  .graph .node.hidden rect,
  .graph .node.hidden text,
  .graph .edgePath.hidden path {
  opacity: 0.2;
  transition: opacity 0.5s;
  }
  .graph .node.dests rect {
  fill: #e1f5fd;
  /*stroke: #3fc0f3;*/
  stroke: #b5eaff;
  }
  .graph .node.dests text {
  fill: #308fb5;
  }

  .graph .node.sources rect {
  fill: #e7fde2;
  /*stroke: #2dc00c;*/
  stroke: #b2f1a4;
  }
  .graph .node.sources text {
  fill: #29a20e;
  }

  .graph .node:hover rect {
  stroke-width: 2px;
  stroke: #333;
  opacity: 0.8;
  transition: opacity 0.1s;
  }

  .graph {
  width: 100%;
  border: 1px solid #EEE;
  }

  #row-description {
  padding-left:2rem;
  }
  #title-parag {
  font-size:16px;
  font-weight: bolder;
  }
</style>
