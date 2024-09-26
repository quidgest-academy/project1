<template>
    <div>
        <row>
            <qtable :columns="Roles.columns"
                    :rows="Roles.rows"
                    :total_rows="Roles.total_rows"
                    :config="Roles.config"
                    @on-single-select-row="viewRole"
                    >
            <template #Designation="props">
                <span class="role-tag">{{$t(props.row.Designation)}}</span>
            </template>
            </qtable>
        </row>
        <row>
            <h1>{{Resources.HIERARQUIA22557}}</h1>
            <div>
                <row>
                    <select class="form-control" style="float:left; width: 200px;" v-model="selectedModule">
                        <option value="" disabled hidden>{{Resources.SELECCIONAR_MODULO15000}}</option>
                        <option v-for="module in this.modules" v-bind:value="module.Module" :key="module.Module">{{ module.ModuleName }}</option>
                    </select>
                    
                </row>
            </div>
        </row>
        <row>
            <div class="col-sm">
                <svg class="graph" ref="svg"><g/></svg>
            </div>
        </row>
    </div>
</template>
<script>
    import { reusableMixin } from '@/mixins/mainMixin';
    import { QUtils } from '@/utils/mainUtils';
    import { PathUtils } from '@/utils/PathFinder';
    import dagreD3 from "dagre-d3";
    import * as d3 from "d3";

    export default {
        name: 'roles',
        mixins: [reusableMixin],
        data: function () {
            var vm = this;
            return {
                modules: [{
                    Module: String,
                    ModuleName: String,
                }],
                Roles: {
                    rows: [],
                    total_rows: 1,
                    columns: [
                        {
                            label: () => vm.$t('MODULO43834'),
                            name: "ModuleName",
                            sort: true,
                            initial_sort: true,
                            initial_sort_order: "asc"
                        },
                        {
                            label: () => vm.$t('ROLE60946'),
                            name: "Designation",
                            sort: true,
                            initial_sort: true,
                            initial_sort_order: "asc"
                        }],
                    config: {
                        single_row_selectable : true
                    }
                },
                graphic: [{
                    name: String,
                    imports: [],
                    class: String
                }],
                selectedModule: ''
            }
        },
        mounted() {
            // access our input using template refs, then focus
            this.$refs.svg.focus()
        },
        methods: {
            fetchRoles: function () {
                var vm = this;
                QUtils.FetchData(QUtils.apiActionURL('Users', 'GetRoles')).done(function (data) {
                    vm.Roles.rows = data;
                    vm.Roles.total_rows = vm.Roles.rows.length;
					
					//get the distinct modules from the list of roles
					vm.getDistinctModules();
                });
            },
            viewRole: function (obj) {
                var row = obj.row;
                this.$router.push({
                    name: 'view_role',
                    params: { role: row.Role, module: row.Module, culture: this.currentLang, system: this.currentYear }
                });
            },
            resetGraph: function () {
                while(this.graphic.length > 1){
                    this.graphic.pop();
                }
            },
            drawGraph: async function () {
                
                this.resetGraph(); // remove the previous graphic before drawing again

                var svg = d3.select(this.$refs.svg);
            
                this.inner = svg.select("g");

                this.g = new dagreD3.graphlib.Graph();

                var graph = this.g;
                graph.setGraph({});

                this.g.graph().rankdir = "TB";
                this.g.graph().ranker = "network-simplex"; //network-simplex, longest-path, tight-tree

                //define the graph and graphic 
                await this.getAllNodeDependencies(graph);

                //define the matrix using the calculated graphic
                this.reach = PathUtils.reachabilityMatrix(this.graphic);
                
                //create the correct edges 
                this.updateEdges();

                // Set up zoom support
                
                this.zoom = d3.zoom().on("zoom", function(e){
                    d3.select('svg g').attr("transform", e.transform);
                });
                    
                svg.call(this.zoom);
                svg.on("dblclick.zoom", null);
        
                // Create the renderer
                this.updateRender();

                //set up node click event
                svg.selectAll("g.node")
                    .on("dblclick", this.nodeDblClickHandler)
                    .on("click", this.nodeClickHandler);

            },
            updateEdges: function() {
           
                //remove all the previous edges
                this.g.edges().forEach( edge=> {
                    this.g.removeEdge(edge.v, edge.w);
                }, this);

                // setup the edges using the calculated graphic
                this.graphic.forEach( i=> {
                    i.imports.forEach( src=> {
                        if(this.g.node(this.$t(src.Designation)))
                        {
                            //the calculated reach is used to check if an edge is redundant
                            if(PathUtils.isPrimaryEdge(this.reach, i.name, this.$t(src.Designation)))
                            {
                                var clEdge = "";
                                var wgEdge = 1;
                            
                                this.g.setEdge(i.name, this.$t(src.Designation), {label:"", curve: d3.curveBasis, class: clEdge, weight: wgEdge });
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

                render(this.inner, this.g);

                //set scale
                this.fitSize();
                
            },
            fitSize: function() {    
                var width = this.$refs.svg.clientWidth;
            
                // Center the graph	
                //var initialScale = 0.75;
                //resize to fit
                var initialScale = (width * 1.0) / (this.g.graph().width + 50.0);
            
                if(initialScale > 0.75)
                    initialScale = 0.75;
                this.rescale(initialScale);
            },       
            rescale: function(newScale) {  
                var svg = d3.select(this.$refs.svg);
                
                var width = this.$refs.svg.clientWidth;
            
                // Center the graph
                this.zoom.transform(svg, d3.zoomIdentity
                    .translate((width - this.g.graph().width * newScale) / 2, 20)
                    .scale(newScale)
                    );
                
                //minimize the height of the container
                svg.attr('height', this.g.graph().height + 40);
                
            },
            nodeDblClickHandler : function(d) {
                var vm = this;
                vm.Roles.rows.forEach(element =>{
                    if(vm.selectedModule == element.Module) {
                        if(this.$t(element.Designation) == d.path[1].__data__){

                            //find the correct role in the correct module
                            this.$router.push({
                                name: 'view_role',
                                params: { role: element.Role, module: element.Module, culture: this.currentLang, system: this.currentYear }
                            });
                        }
                    }
                })
            },
            getAllNodeDependencies: async function (graph) {
                var vm = this;

                //we set each node in the graph
                //also, we fetch the children of each node in the module and define the corresponding graphic
                for(const element of vm.Roles.rows) {
                    if(vm.selectedModule == element.Module) {

                        graph.setNode(this.$t(element.Designation), {label: this.$t(element.Designation), class: "origin"});

                        QUtils.log("Fetch data - Role");
                        var data = await QUtils.FetchData(QUtils.apiActionURL('Role', 'GetRole', { module: vm.selectedModule, roleId: element.Role }));
                            QUtils.log("Fetch data - OK (Role)");
                        
                            //define the correct designation of each node
                            data.Children.forEach(child => {
                                child.Designation = vm.$t(child.Designation)
                            })

                            //add the node( with its children) to the graphic
                            vm.graphic.push({
                                name: vm.$t(data.Designation),
                                imports: data.Children,
                                class: "origin"
                            });
                    }
                }
                this.graphic.shift();
            },
            getDistinctModules: function () {
                var vm = this;
                vm.modules = vm.Roles.rows.filter((value, index, self) =>
                    index === self.findIndex((t) => (
                        t.Module === value.Module && t.ModuleName === value.ModuleName
                    ))
                )
            }
        },
        created: function () {
            // Ler dados
            this.fetchRoles();
        },
        watch: {
            // call again the method if the route changes
            '$route': 'fetchRoles',
            'selectedModule': 'drawGraph'
        }
    }  
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
        transition: opacity 0.1;
    }

    .graph {
        width: 100%;
        border: 1px solid #EEE;
    }
</style>
