<template>
    <!-- TODO configurable header title position -->
    <div :class="{card:card_mode}">
        <div class="card-header" v-if="card_mode">
            <slot name="card-header">
                <template>
                    {{card_title}}
                </template>
            </slot>
        </div>
        <div :class="{'card-body':card_mode}">
          <div class="row" v-if="getTableTitle || showToolsRow">
            <div class="col" v-if="getTableTitle">
              <legend class="form-header">
                {{getTableTitle()}}
              </legend>
            </div>
            <template v-if="showToolsRow">
              <!-- global search text starts here -->
                <search :initPlaceholder="global_search.placeholder"
                        :initClasses="global_search.classes"
                        :initVisibility="global_search.visibility"
                        :initCaseSensitive="global_search.caseSensitive"
                        :initShowClearButton="global_search.showClearButton"
                        :initSearchOnPressEnter="global_search.searchOnPressEnter"
                        :initSearchDebounceRate="global_search.searchDebounceRate"
                        :initShowRefreshButton="global_search.showRefreshButton"
                        :initShowResetButton="global_search.showResetButton"
                        @clearGlobalSearch="clearGlobalSearch"
                        @updateGlobalSearchHandler="updateGlobalSearchHandler"
                        @updateGlobalSearch="updateGlobalSearch"
                        @emitSearch="emitSearch"
                        @resetQuery="resetQuery">
                </search>
              <!-- global search text ends here -->
              
              <!-- export to excel button -->
              <div v-if="enableExport" class="col-md-auto my-auto">
                <button class="form-group b-icon-text b-icon-text--primary" @click="exportExcel">
                    {{ exportLabel }}
                </button>
              </div>
              <!--  export to excel button ends here -->

              <!-- action buttons starts here -->
              <div class="col-md-8">
                <slot name="vbt-action-buttons">
                  <div class="btn-group float-right" role="group" aria-label="Basic example">
                    <button v-for="(action, ac_key, index) in actions"
                            :key="index" type="button" class="btn"
                            :class="getActionButtonClass(action)"
                            @click="emitActionEvent(action)">
                      <slot :name="action.btn_text_slot_name">
                        <span v-html="action.btn_text"></span>
                      </slot>
                    </button>
                  </div>
                </slot>
              </div>
              <!-- action buttons button ends here -->
            </template>
          </div>

          <div :class='tableWrapperClasses' class="vbt-table-wrapper">
            <table class="c-table" :class="tableClasses">
              <thead class="c-table__head">
                <tr>
                  <select-all-rows-check-box v-if="checkbox_rows"
                                             :all-rows-selected="allRowsSelected"
                                             @update:allRowsSelected="allRowsSelected = $event"
                                             :current-page-selection-count="currentPageSelectionCount"
                                             @select-all-row-checkbox="selectAllCheckbox" />
                  <slot name="columns" :columns="vbt_columns">
                    <template v-for="(column, cl_key, index) in vbt_columns" :key="index">
                      <th v-if="canShowColumn(column)" v-on="isSortableColumn(column) ? { click: () => updateSortQuery(column) } : {}" class="vbt-column-header" :class="columnClasses(column)">
                        <slot :name="'column_' + getCellSlotName(column)" :column="column">
                          {{getColumnLabel(column)}}
                        </slot>
                        <template v-if='isSortableColumn(column)'>
                          <SortIcon :sort="query.sort" :column="column">
                            <template #vbt-sort-asc-icon>
                              <slot name="sort-asc-icon">
                                &#x1F825;
                              </slot>
                            </template>
                            <template #vbt-sort-desc-icon>
                              <slot name="sort-desc-icon">
                                &#x1F827;
                              </slot>
                            </template>
                            <template #vbt-no-sort-icon>
                              <slot name="no-sort-icon">
                                &#x1F825;&#x1F827;
                              </slot>
                            </template>
                          </SortIcon>
                        </template>
                      </th>
                    </template>
                  </slot>
                </tr>
              </thead>
              <tbody class="c-table__body">
                <!-- filter row starts here -->
                <tr class="filtersRow" v-if="showFilterRow">
                  <td v-show="checkbox_rows"></td>
                  <template v-for="(column, cl_key, index) in vbt_columns" :key="index">
                    <td v-if="canShowColumn(column)" align="center">
                      <template v-if="hasFilter(column)">
                        <Simple v-if="column.filter.type == 'simple'" :column="column" @update-filter="updateFilter" @clear-filter="clearFilter">
                          <template #vbt-simple-filter-clear-icon>
                            <slot name="simple-filter-clear-icon">
                              &#x24E7;
                            </slot>
                          </template>
                        </Simple>
                        <MultiSelect v-if="column.filter.type == 'select'" :options="column.filter.options" :column="column" @update-multi-select-filter="updateMultiSelectFilter" @clear-filter="clearFilter"></MultiSelect>
                        <template v-if="column.filter.type == 'custom'">
                          <slot :name="column.filter.slot_name" :column="column">
                          </slot>
                        </template>
                      </template>
                    </td>
                  </template>
                </tr>
                <!-- filter row ends here -->
                <!-- data rows stars here -->
                <row v-for="(row, index) in vbt_rows" :key="index"
                     :row="row"
                     :columns="vbt_columns"
                     :row-index="index"
                     :checkbox-rows="checkbox_rows"
                     :rows-selectable="rows_selectable"
                     :single-row-selectable="single_row_selectable"
                     :selected-items="selected_items"
                     :highlight-row-hover="highlight_row_hover"
                     :highlight-row-hover-color="rowHighlightColor"
                     :prop-row-classes="classes.row"
                     :prop-cell-classes="classes.cell"
                     :unique-id="uniqueId"
                     @add-row="handleAddRow"
                     @remove-row="handleRemoveRow"
                     @single-row-select="handleSingleRowSelect">
                  <template v-for="(column) in columns" #[`vbt-${getCellSlotName(column)}`]>
                    <slot :name="getCellSlotName(column)" :row="row" :column="column" :cell_value="getValueFromRow(row,column.name)">
                      {{getValueFromRow(row,column.name)}}
                    </slot>
                  </template>
                </row>
                <!-- empty row starts here -->
                <tr v-show="vbt_rows == 0">
                  <td :colspan="headerColSpan">
                    <slot name="empty-results">
                      No results found
                    </slot>
                  </td>
                </tr>
                <!-- empty row ends here -->
                <!-- data rows ends here -->
                <!-- Pagination row starts here -->
                <tr v-if="showPaginationRow" class="footer-pagination-row">
                  <td :colspan="headerColSpan">
                    <div class="row vbt-pagination-row no-gutters">
                      <!-- pagination starts here -->
                      <div class="col-md-8">
                        <div v-if="pagination">
                          <Pagination v-model:page="page" v-model:per_page="per_page" :per_page_options="per_page_options" :total="rowCount" :num_of_visibile_pagination_buttons="num_of_visibile_pagination_buttons">
                            <template #vbt-paginataion-previous-button>
                              <slot name="paginataion-previous-button">
                                &laquo;
                              </slot>
                            </template>
                            <template #vbt-paginataion-next-button>
                              <slot name="paginataion-next-button">
                                &raquo;
                              </slot>
                            </template>
                          </Pagination>
                        </div>
                      </div>
                      <!-- pagination ends here -->
                      <!-- pagination info start here -->
                      <div class="col-md-4">
                        <div class="text-right justify-content-center">
                          <template v-if="pagination_info">
                            <slot name="pagination-info" :currentPageRowsLength="currentPageRowsLength" :filteredRowsLength="filteredRowsLength" :originalRowsLength="originalRowsLength">
                              <template v-if="currentPageRowsLength != 0">
                                From 1 to {{currentPageRowsLength}} of {{filteredRowsLength}} entries
                              </template>
                              <template v-else>
                                No results found
                              </template>
                              <template>
                                ({{originalRowsLength}} total records)
                              </template>
                            </slot>
                          </template>
                          <template v-if="selected_rows_info && pagination_info && isSelectable">
                            <slot name="pagination-selected-rows-separator">
                              |
                            </slot>
                          </template>
                          <template v-if="selected_rows_info && isSelectable">
                            <slot name="selected-rows-info" :selectedItemsCount="selectedItemsCount">
                              {{selectedItemsCount}} rows selected
                            </slot>
                          </template>
                        </div>
                      </div>
                      <!-- pagination info ends here -->
                    </div>
                  </td>
                </tr>
                <!-- Pagination ends starts here -->
              </tbody>
              <tfoot class="c-table__footer">
                <slot name="table-footer"></slot>
              </tfoot>
            </table>
          </div>
        </div>
        <div class="card-footer" v-if="card_mode">
            <slot name="card-footer">
                <div class="row">
                    <!-- pagination starts here -->
                    <div class="col-md-6">
                        <div v-if="pagination">
                            <Pagination v-model:page="page" v-model:per_page="per_page" :per_page_options="per_page_options" :total="rowCount" :num_of_visibile_pagination_buttons="num_of_visibile_pagination_buttons">
                                <template #vbt-paginataion-previous-button>
                                    <slot name="paginataion-previous-button">
                                        &laquo;
                                    </slot>
                                </template>
                                <template #vbt-paginataion-next-button>
                                    <slot name="paginataion-next-button">
                                        &raquo;
                                    </slot>
                                </template>
                            </Pagination>
                        </div>
                    </div>
                    <!-- pagination ends here -->

                    <!-- pagination info start here -->
                    <div class="col-md-6">
                        <div class="text-right justify-content-center">
                            <template v-if="pagination_info">
                                <slot name="pagination-info" :currentPageRowsLength="currentPageRowsLength" :filteredRowsLength="filteredRowsLength" :originalRowsLength="originalRowsLength">
                                    <template v-if="currentPageRowsLength != 0">
                                        From 1 to {{currentPageRowsLength}} of {{filteredRowsLength}} entries
                                    </template>
                                    <template v-else>
                                        No results found
                                    </template>
                                    <template>
                                        ({{originalRowsLength}} total records)
                                    </template>
                                </slot>
                            </template>
                            <template v-if="pagination_info && selected_rows_info">
                                <slot name="pagination-selected-rows-separator">
                                    |
                                </slot>
                            </template>
                            <template v-if="selected_rows_info">
                                <slot name="selected-rows-info" :selectedItemsCount="selectedItemsCount">
                                    {{selectedItemsCount}} rows selected
                                </slot>
                            </template>
                        </div>
                    </div>
                    <!-- pagination info ends here -->
                </div>
            </slot>
        </div>
    </div>
</template>

<script>

import {
findIndex,
range,
filter,
includes,
map,
join,
has,
extend,
isEmpty,
isEqual,
debounce,
cloneDeep,
differenceWith,
differenceBy,
intersectionWith,
intersectionBy,
orderBy,
get,
omit,
clone
} from "lodash-es";

import Search from "./Search.vue";
import Row from "./Row.vue";
//import CheckBox from "./CheckBox.vue";
import SelectAllRowsCheckBox from "./SelectAllRowsCheckBox.vue";
import SortIcon from "./SortIcon.vue";
import Pagination from "./Pagination.vue";
import Simple from "./Filters/Simple.vue";
import MultiSelect from "./Filters/MultiSelect.vue";

  import { reusableMixin } from '@/mixins/mainMixin.js';

// SOURCE: https://github.com/rubanraj54/vue-bootstrap4-table

import { QUtils } from '@/utils/mainUtils';

export default {
    name: "QTable",
    mixins: [reusableMixin],
    emits: ['on-select-row', 'on-unselect-row', 'on-all-select-rows', 'on-all-unselect-rows', 'on-single-select-row', 'on-change-query'],
    props: {
        rows: {
            type: Array,
            default: function () {
              return [];
            }
        },
        columns: {
            type: Array,
            required: true
        },
        totalRows: {
            type: Number,
            default: 0
        },
        config: {
            type: Object,
            default: function () {
                return {};
            }
        },
        classes: {
            type: Object,
            default: function () {
                return {};
            }
        },
        actions: {
            type: Array,
            default: function () {
                return [];
            }
        },
        customFilters: {
            type: Array,
            default: function () {
                return [];
            }
        },
        enableExport: {
            type: Boolean,
            default: false
        },
        exportLabel: {
            type: String,
            default: ''
        }
    },
    data: function () {
        return {
            vbt_rows: [],
            vbt_columns: [],
            query: {
                sort: [],
                filters: [],
                global_search: ""
            },
            page: 1,
            per_page: 10,
            original_rows: [],
            num_of_visibile_pagination_buttons: 5,
            temp_filtered_results: [],
            pagination: true,
            pagination_info: true,
            checkbox_rows: false,
            selected_items: [],
            highlight_row_hover: true,
            highlight_row_hover_color: "#d6d6d6",
            rows_selectable: false,
            single_row_selectable: false,
            allRowsSelected: false,
            multi_column_sort:false,
            card_title: "",
            table_title: "",
            global_search: {
                placeholder: "Enter search text",
                classes: "",
                visibility: true,
                caseSensitive: false,
                showRefreshButton: true,
                showResetButton: true,
                showClearButton: false,
                searchOnPressEnter: true,
                searchDebounceRate: 60,
                init: {
                    value: ""
                }
            },
            per_page_options : [5,10,15],
            server_mode: false,
            total_rows: 0,
            card_mode: false,
            selected_rows_info: false,
            lastSelectedItemIndex: null,
            isFirstTime: true,
            isResponsive: false,
            preservePageOnDataChange: false,
            canEmitQueries : false,
        };
        },
    created() {
        this.initConfig();
    },
    mounted() {
        this.vbt_rows = cloneDeep(this.rows);
        this.vbt_columns = cloneDeep(this.columns);
        let self = this;
        // check if user mentioned unique id for a column, if not set unique id for all items
        this.original_rows = map(this.vbt_rows, function (element, index) {
            let extra = {};
            if (!self.hasUniqueId) {
                extra.vbt_id = index + 1;
            }
            return extend({}, element, extra);
        });

        this.vbt_columns = map(this.vbt_columns, function (element, index) {
            let extra = {};
            extra.vbt_col_id = index + 1;
            return extend({}, element, extra);
        });

        //this.initConfig();
        this.initialSort();
        this.initFilterQueries();

        if (this.global_search.visibility) {
            this.$nextTick(() => {
                this.initGlobalSearch()
            });
        }

        this.$nextTick(() => {
            if (!this.server_mode) {
                this.filter(false,true);
            } else {
                this.canEmitQueries = true;
                this.emitQueryParams();
            }
        });

        this.handleShiftKey();

    },
    components: {
        Search,
        Row,
        //CheckBox,
        SelectAllRowsCheckBox,
        Simple,
        MultiSelect,
        SortIcon,
        Pagination,
    },
    methods: {
        initConfig() {

            if (isEmpty(this.config)) {
                return;
            }

            this.pagination = (has(this.config, 'pagination')) ? this.config.pagination : true;

            this.num_of_visibile_pagination_buttons = (has(this.config, 'num_of_visibile_pagination_buttons')) ? this.config.num_of_visibile_pagination_buttons : 7;

            this.per_page_options = (has(this.config, 'per_page_options')) ? this.config.per_page_options : [5,10,15];

            this.per_page = (has(this.config, 'per_page')) ? this.config.per_page : 10;

            this.page = (has(this.config, 'page')) ? this.config.page : 1;

            this.checkbox_rows = (has(this.config, 'checkbox_rows')) ? this.config.checkbox_rows : false;

            this.highlight_row_hover = (has(this.config, 'highlight_row_hover')) ? this.config.highlight_row_hover : true;

            this.highlight_row_hover_color =  (has(this.config, 'highlight_row_hover_color')) ? this.config.highlight_row_hover_color : "#d6d6d6";

            this.rows_selectable = (has(this.config, 'rows_selectable')) ? this.config.rows_selectable : false;

            this.single_row_selectable = (has(this.config, 'single_row_selectable')) ? this.config.single_row_selectable : false;

            this.multi_column_sort = (has(this.config, 'multi_column_sort')) ? (this.config.multi_column_sort) : false;

            this.pagination_info = (has(this.config, 'pagination_info')) ? this.config.pagination_info : true;

            this.card_title = (has(this.config, 'card_title')) ? this.config.card_title : "";

            this.table_title = (has(this.config, 'table_title')) ? this.config.table_title : "";

            if (has(this.config, 'global_search')) {
                this.global_search.placeholder = (has(this.config.global_search, 'placeholder')) ? this.config.global_search.placeholder : "Enter search text";
                this.global_search.visibility = (has(this.config.global_search, 'visibility')) ? this.config.global_search.visibility : true;
                this.global_search.caseSensitive = (has(this.config.global_search, 'caseSensitive')) ? this.config.global_search.caseSensitive : false;
                this.global_search.showRefreshButton = (has(this.config.global_search, 'showRefreshButton')) ? (this.config.global_search.showRefreshButton) : true;
                this.global_search.showResetButton = (has(this.config.global_search, 'showResetButton')) ? (this.config.global_search.showResetButton) : true;
                this.global_search.showClearButton = (has(this.config.global_search, 'showClearButton')) ? this.config.global_search.showClearButton : false;
                this.global_search.searchOnPressEnter = (has(this.config.global_search, 'searchOnPressEnter')) ? this.config.global_search.searchOnPressEnter : true;
                this.global_search.searchDebounceRate = (has(this.config.global_search, 'searchDebounceRate')) ? this.config.global_search.searchDebounceRate : 60;
                this.global_search.classes = (has(this.config.global_search, 'classes')) ? this.config.global_search.classes : "";
                this.global_search.init.value = (has(this.config.global_search, 'init.value')) ? this.config.global_search.init.value: "";
            }

            this.server_mode = (has(this.config, 'server_mode')) ? (this.config.server_mode) : false;

            this.card_mode = (has(this.config, 'card_mode')) ? (this.config.card_mode) : false;

            this.selected_rows_info = (has(this.config, 'card_mode')) ? (this.config.selected_rows_info) : false;

            this.preservePageOnDataChange = (has(this.config, 'preservePageOnDataChange')) ? (this.config.preservePageOnDataChange) : false;

        },

        initialSort() {
            // TODO optimze this with removing this filter
            let initial_sort_columns =  filter(this.vbt_columns, column => (has(column,'initial_sort') && column.initial_sort == true));

            initial_sort_columns.some(initial_sort_column => {

                let result = findIndex(this.query.sort, { 'vbt_col_id': initial_sort_column.vbt_col_id });

                if(result == -1) {
                    // initial sort order validation starts here
                    let initial_sort_order = "asc";
                    if (has(initial_sort_column,"initial_sort_order")) {
                        if (includes(["asc","desc"], initial_sort_column.initial_sort_order)) {
                            initial_sort_order = initial_sort_column.initial_sort_order;
                        } else {
                            console.log("invalid initial_sort_order, so setting it to default");
                        }
                    }
                    // initial sort order validation ends here
                    this.query.sort.push({
                        vbt_col_id: initial_sort_column.vbt_col_id,
                        name: initial_sort_column.name,
                        order: initial_sort_order,
                        caseSensitive: this.isSortCaseSensitive(initial_sort_column)
                    });

                }
                // else {
                //     this.query.sort[result].order = initial_sort_column.initial_sort_order;
                // }

                // if multicolum sort sort is false, then consider only first initial sort column
                if (!this.multi_column_sort) {
                    return true;
                }
            });
        },

        initGlobalSearch() {
            //this.$refs.global_search.value = this.global_search.init.value;
            this.query.global_search = this.global_search.init.value;
        },

        hasFilter(column) {
            return has(column, "filter.type");
        },

        clearFilter(column) {
            let filter_index = this.getFilterIndex(column);
            if (filter_index !== -1) {
                this.query.filters.splice(filter_index, 1);
            }
        },

        getFilterIndex(column) {
            return findIndex(this.query.filters, {
                name: column.name
            });
        },

        initFilterQueries() {
            this.vbt_columns.forEach(vbt_column => {


                if (!has(vbt_column,'filter')) return;

                if (vbt_column.filter.type == "simple") {
                    if (!has(vbt_column,'filter.init.value')) return;

                    this.updateFilter({
                        "value" : vbt_column.filter.init.value,
                        "column" : vbt_column
                    });
                } else if (vbt_column.filter.type == "select") {

                    if (!has(vbt_column,'filter.init.value')) return;

                    let initialValues = [];
                    if (vbt_column.filter.mode == "multi") {
                        if (Array.isArray(vbt_column.filter.init.value)) {
                            initialValues = vbt_column.filter.init.value;
                        } else {
                            console.log("Initial value for 'multi' mode should be an array");
                        }
                    } else if (vbt_column.filter.mode == "single") {
                        if (Number.isInteger(vbt_column.filter.init.value) && vbt_column.filter.init.value > -1) {
                            initialValues = [vbt_column.filter.init.value];
                        } else {
                            console.log("Initial value for 'single' mode should be a single number and greater than -1");
                        }
                    }

                    let selected_options = vbt_column.filter.options.filter((_,index) => includes(initialValues, index)).map(filtered_option => filtered_option.value);

                    this.updateMultiSelectFilter({
                        "selected_options" : selected_options,
                        "column" : vbt_column
                    });
                }
            });
        },

        isSortCaseSensitive(column) {
            return (column.sortCaseSensitive != undefined) ? column.sortCaseSensitive : true;
        },

        updateSortQuery(column) {

            let result = findIndex(this.query.sort, { 'vbt_col_id': column.vbt_col_id });

            if(result == -1) {

                if (!this.multi_column_sort) {
                    this.query.sort = [];
                }
                this.query.sort.push({
                    vbt_col_id: column.vbt_col_id,
                    name: column.name,
                    order: "asc",
                    caseSensitive: this.isSortCaseSensitive(column)
                });

            } else {
                this.query.sort[result].order = this.query.sort[result].order == "asc" ? "desc" : "asc";
            }
        },
        isShiftSelection(shiftKey,rowIndex){
            return (shiftKey == true) && (this.lastSelectedItemIndex != null) && (this.lastSelectedItemIndex != rowIndex);
        },
        handleAddRow(payload) {
            let row = this.vbt_rows[payload.rowIndex];
            if (this.isShiftSelection(payload.shiftKey,payload.rowIndex)) {
                let rows = this.getShiftSelectionRows(payload.rowIndex);
                rows.forEach((_row) => {this.addSelectedItem(_row)});
            } else {
                this.addSelectedItem(row);
            }

            this.$emit('on-select-row', {"selected_items":cloneDeep(this.selected_items) ,"selected_item":row});

            let difference = [];

            if (this.server_mode && !this.hasUniqueId) {
                difference = differenceWith(this.vbt_rows, this.selected_items, isEqual);
            } else {
                difference = differenceBy(this.vbt_rows, this.selected_items, this.uniqueId);
            }

            if (difference.length == 0) {
                this.allRowsSelected = true;
                // this.$eventHub.emit('select-select-all-items-checkbox', "from main");
            } else {
                this.allRowsSelected = false;
                // this.$eventHub.emit('unselect-select-all-items-checkbox', "from main");
            }

            this.lastSelectedItemIndex = payload.rowIndex;
        },

        getActionButtonClass(action){
            return has(action,'class') ? action.class : " btn-secondary";
        },

        handleRemoveRow(payload) {
            let row = this.vbt_rows[payload.rowIndex];
            if (this.isShiftSelection(payload.shiftKey,payload.rowIndex)) {
                let rows = this.getShiftSelectionRows(payload.rowIndex);
                rows.forEach((_row) => {this.removeSelectedItem(_row)});
            } else {
                this.removeSelectedItem(row);
            }
            this.$emit('on-unselect-row', {"selected_items":cloneDeep(this.selected_items),"unselected_item":row});
            // this.$eventHub.emit('unselect-select-all-items-checkbox');
            this.allRowsSelected = false;
            this.lastSelectedItemIndex = payload.rowIndex;
        },
        addSelectedItem(item) {

            let index = -1;
            if (this.server_mode && !this.hasUniqueId) {
                index = findIndex(this.selected_items, (selected_item) => {return isEqual(selected_item, item)});
            } else {
                index = findIndex(this.selected_items, (selected_item) => {return selected_item[this.uniqueId] == item[this.uniqueId]});
            }

            if (index == -1) {
                this.selected_items.push(item);
            }
        },
        selectAllItems() {

            let difference = [];

            if (this.server_mode && !this.hasUniqueId) {
                difference = differenceWith(this.vbt_rows, this.selected_items, isEqual);
            } else {
                difference = differenceBy(this.vbt_rows, this.selected_items, this.uniqueId);
            }

            this.selected_items.push(...difference);

            this.$emit('on-all-select-rows', {"selected_items":cloneDeep(this.selected_items) });

        },
        unSelectAllItems() {

            let difference = [];

            if (this.server_mode && !this.hasUniqueId) {
                let result = intersectionWith(this.vbt_rows, this.selected_items, isEqual);
                difference = differenceWith(this.selected_items, result, isEqual);
            } else {
                let result = intersectionBy(this.vbt_rows, this.selected_items, this.uniqueId);
                difference = differenceBy(this.selected_items, result, this.uniqueId);
            }

            this.selected_items = difference;

            this.$emit('on-all-unselect-rows', {"selected_items":cloneDeep(this.selected_items)});

        },
        removeSelectedItem(item) {
            // TODO try with findbyId function
            this.selected_items.some((selected_item,index) => {
                if (isEqual(item, selected_item)) {
                    this.selected_items.splice(index, 1);
                    return true;
                }
            });
        },
        getShiftSelectionRows(rowIndex) {
            let start = 0;
            let end = 0;
            if (this.lastSelectedItemIndex < rowIndex) {
                start = this.lastSelectedItemIndex;
                end = rowIndex + 1;
            } else if (this.lastSelectedItemIndex > rowIndex) {
                start = rowIndex;
                end = this.lastSelectedItemIndex + 1;
            }
            return this.vbt_rows.slice(start,end);
        },

        handleSingleRowSelect(payload) {
            let row = this.vbt_rows[payload.rowIndex];
            this.$emit('on-single-select-row', { row, rowIndex: payload.rowIndex });
        },
        
        updateFilter(payload) {
            let value = (typeof payload.value == "number") ? payload.value.toString() : payload.value;
            let column = payload.column;
            let filter_index = findIndex(this.query.filters, {
                name: column.name
            });
            if (filter_index == -1) {
                if (value !== "") {
                    this.query.filters.push({
                        type: column.filter.type,
                        name: column.name,
                        text: value.trim(),
                        config: column.filter
                    });
                }
            } else {
                if (value === "") {
                    this.query.filters.splice(filter_index, 1);
                } else {
                    this.query.filters[filter_index].text = value.trim();
                }
            }
        },
        updateMultiSelectFilter(payload) {

            let selected_options = payload.selected_options;
            let column = payload.column;


            let filter_index = findIndex(this.query.filters, {
                name: column.name
            });

            if (filter_index == -1) {
                if (selected_options.length === 0) {
                    return;
                }
                this.query.filters.push({
                    type: column.filter.type,
                    mode: column.filter.mode,
                    name: column.name,
                    selected_options: selected_options,
                    config: column.filter
                });
            } else {
                if (selected_options.length === 0) {
                    this.query.filters.splice(filter_index, 1);
                } else {
                    this.query.filters[filter_index].selected_options = selected_options;
                }
            }
        },

        sort() {

            if(this.query.sort.length != 0) {
                let orders = this.query.sort.map(sortConfig => sortConfig.order);

                this.temp_filtered_results = orderBy(this.temp_filtered_results,
                    this.query.sort.map(sortConfig => {
                        return row => {
                            let value = get(row,sortConfig.name);
                            if (sortConfig.caseSensitive) return value != null ? value : '';
                            return value != null ? value.toString().toLowerCase() : '';
                        }
                    }),
                    orders
                );
            }

            this.paginateFilter();
        },

        filter(resetPage = true, isInit = false) {
            let res = filter(this.original_rows, (row) => {
                let flag = true;
                this.query.filters.some((filter, key) => {
                    if (filter.type === "simple") {
                        if (this.simpleFilter(get(row, filter.name), filter.text,filter.config)) {
                            // continue to next filter
                            flag = true;
                        } else {
                            // stop here and break loop since one filter has failed
                            flag = false;
                            return true;
                        }
                    } else if (filter.type === "select") {
                        if (this.multiSelectFilter(get(row, filter.name), filter.selected_options,filter.config)) {
                            flag = true;
                        } else {
                            flag = false;
                            return true;
                        }
                    } else if (filter.type === "custom") {
                        let index = findIndex(this.vbt_columns,{name:filter.name});
                        if (index > -1) {
                            let column = this.vbt_columns[index];
                            if (column.filter.validator) {
                                let result = column.filter.validator(get(row, filter.name),filter.text);
                                if (result == true || result == undefined) {
                                    flag = true;
                                } else {
                                    flag = false;
                                    return true;
                                }
                            } else {
                                flag = true;
                            }
                        } else {
                            flag = true;
                        }
                    }
                });
                return flag;
            });

            this.temp_filtered_results = res;

            // Do global search only if global search text is not empty and
            // filtered results is also not empty
            if (this.query.global_search !== "" && this.rowCount != 0 ) {
                this.temp_filtered_results = this.globalSearch(this.temp_filtered_results);
            }

            this.sort();
            if (resetPage || this.rowCount == 0) {
                this.page = 1;
            } else if (!isInit) {
                let newTotalPage = Math.ceil(this.rowCount/this.per_page);
                this.page = (this.page <= newTotalPage) ? this.page : newTotalPage;
            }
        },

        globalSearch(temp_filtered_results) {
            let global_search_results = filter(temp_filtered_results, (row) =>{

                let flag = false;

                this.vbt_columns.some((vbt_column, key) => {
                    let value = get(row, vbt_column.name);
                    let global_search_text = this.query.global_search;

                    if (value == null || typeof value === "undefined") {
                        value =  "";
                    }

                    if (typeof value !== "string") {
                        value = value.toString();
                    }

                    if (typeof global_search_text !== "string") {
                        global_search_text = global_search_text.toString();
                    }

                    if (!this.global_search.caseSensitive) {
                        value = value.toLowerCase();
                        global_search_text = global_search_text.toLowerCase();
                    }

                    if (value.indexOf(global_search_text) > -1) {
                        flag = true;
                        return;
                    }

                });

                return flag;
            });

            return global_search_results;
        },

        simpleFilter(value, filter_text,config) {

            if (value == null || typeof value === "undefined") {
                value =  "";
            }

            if (typeof value !== "string") {
                value = value.toString();
            }

            if (typeof filter_text !== "string") {
                value = filter_text.toString();
            }

            let is_case_sensitive = (has(config,'case_sensitive')) ? config.case_sensitive : false;

            if (!is_case_sensitive) {
                value = value.toLowerCase();
                filter_text = filter_text.toLowerCase();
            }

            return value.indexOf(filter_text) > -1;
        },
        multiSelectFilter(value, selected_options,config) {

            if (typeof value !== "string") {
                value = value.toString().toLowerCase();
            } else {
                value = value.toLowerCase();
            }

            selected_options = map(selected_options, (option) => {
                                    return (typeof option !== "string") ? option.toString().toLowerCase() : option.toLowerCase();
                                });
            return includes(selected_options, value);
            // let is_case_sensitive = (has(config,'case_sensitive')) ? config.case_sensitive : false;

            // if (!is_case_sensitive) {
            //     value = value.toLowerCase();
            //     filter_text = filter_text.toLowerCase();
            // }

            // return value.indexOf(filter_text) > -1;
        },

        paginateFilter() {
            if (this.pagination) {
                let start = (this.page - 1) * this.per_page;
                let end = start + this.per_page;
                this.vbt_rows = this.temp_filtered_results.slice(start, end);
            } else {
                this.vbt_rows = cloneDeep(this.temp_filtered_results);
            }
        },

        selectAllCheckbox() {
            if (this.allRowsSelected || this.currentPageSelectionCount > 0) {
                this.unSelectAllItems();
                this.allRowsSelected = false;
            } else {
                this.selectAllItems();
                this.allRowsSelected = true;
            }
        },

        isSortableColumn(column) {
            if (!has(column,'sort')) {
                return false;
            } else {
                return column.sort;
            }
        },
        // row method starts here
        getValueFromRow(row, name) {
            return get(row, name);
        },
        getCellSlotName(column) {
            if (has(column,"slot_name")) {
                return column.slot_name;
            }
            return column.name.replace(/\./g,'_');
        },
        // row method ends here
        resetSort() {
            this.query.sort = [];
            this.filter(!this.preservePageOnDataChange);
        },

        updateGlobalSearchHandler: function(value) {
            this.query.global_search = value;
        },

        clearGlobalSearch() {
            this.query.global_search = "";
            //this.$refs.global_search.value = "";
        },

        resetQuery() {

            this.query = {
                sort: [],
                filters: [],
                global_search: ""
            }

            //this.$refs.global_search.value = "";
            this.$eventHub.emit('reset-query');

        },
        emitSearch(search_value) {
            //this.query.global_search = this.$refs.global_search.value;
            this.query.global_search = search_value;
        },  

        emitQueryParams(page = null) {
            if (this.server_mode && this.canEmitQueries) {
                let queryParams = cloneDeep(this.query);
                let sort = map(queryParams.sort, o => omit(o, 'vbt_col_id'));
                let filters = map(queryParams.filters, o => omit(o, 'config'));
                let global_search = queryParams.global_search;
                let per_page = clone(this.per_page);

                if (page == null) {
                    if (this.preservePageOnDataChange) {
                        page = this.page;
                    } else {
                        this.page = 1;
                        page = 1;
                    }
                }

                let payload = {
                    sort : sort,
                    filters : filters,
                    global_search : global_search,
                    per_page : per_page,
                    page: page
                }

                this.$emit('on-change-query',payload);
            }
        },
        columnClasses(column) {
            let classes = "";

            let default_text_alignment = "text-left";

            //decide text alignment class - starts here
            let alignments = ["text-justify","text-right","text-left","text-center"];
            if (has(column, "column_text_alignment") && includes(alignments, column.column_text_alignment)) {
                classes = classes + " " + column.column_text_alignment;
            } else {
                classes = classes + " " + default_text_alignment;
            }
            //decide text alignment class - ends here

            // adding user defined classes to rows - starts here
            if (has(column, "column_classes")) {
                classes = classes + " " + column.column_classes;
            }
            // adding user defined classes to rows - ends here

            // adding classes for sortable column - starts here
            if (this.isSortableColumn(column)) {
                classes = classes + " vbt-sort-cursor";
            }
            // adding classes for sortable column - ends here

            return classes;
        },

        getColumnLabel(column) {
            if (typeof column.label === 'function') { return column.label(); }
            else { return column.label; }
        },

        getTableTitle() {
            if (typeof this.table_title === 'function') { return this.table_title(); }
            else { return this.table_title; }
            
        },

        handleShiftKey() {
            ["keyup","keydown"].forEach((event) => {
                window.addEventListener(event, (e) => {
                    document.onselectstart = function() {
                        return !(e.key == "Shift" && e.shiftKey == true);
                    }
                });
            });
        },
        emitActionEvent(action) {
            let payload = {
                event_payload : cloneDeep(action.event_payload)
            }

            if (this.isSelectable) {
                payload.selectedItems = cloneDeep(this.selected_items);
            }

            this.$emit(action.event_name,payload);
        },
        canShowColumn(column) {
            return (column.visibility == undefined || column.visibility) ? true : false;
        },

        exportExcel() {                        
            //Download file
            window.location.href = QUtils.apiActionURL('Users', 'ExportToExcel');
        }
    },
    computed: {
        rowCount() {
            if (!this.server_mode) {
                return this.temp_filtered_results.length;
            } else {
                return this.totalRows;
            }
        },
        selectedItemsCount() {
            return this.selected_items.length;
        },
        filteredResultsCount() {
            return this.temp_filtered_results.length;
        },
        uniqueId() {
            let unique_id = "";

            if (!this.hasUniqueId) {
                unique_id = "vbt_id";
                return unique_id;
            }
            this.vbt_columns.some((column, key) => {
                if (has(column, 'uniqueId') && column.uniqueId === true) {
                    unique_id = column.name;
                    return true;
                }
            });

            return unique_id;
        },
        hasUniqueId() {
            let has_unique_id = false;

            this.vbt_columns.some((column, key) => {
                if (has(column, 'uniqueId') && column.uniqueId === true) {
                    has_unique_id = true;
                    return true;
                }
            });

            return has_unique_id;
        },

        // pagination info computed properties - start

        currentPageRowsLength() {
            return this.vbt_rows.length;
        },

        filteredRowsLength() {
            return this.rowCount;
        },

        originalRowsLength() {
            return (this.server_mode)? this.rowCount : this.rows.length;
        },

        // pagination info computed properties - end
        rowHighlightColor() {
            return (this.highlight_row_hover) ? this.highlight_row_hover_color : "";
        },

        headerColSpan() {
            let count = (this.checkbox_rows) ? 1 : 0;

            count += this.vbt_columns.filter(column => this.canShowColumn(column)).length;

            return count;
        },

        showToolsRow() {
            return (this.global_search.visibility == true || this.global_search.showRefreshButton == true || this.global_search.showResetButton == true || this.actions.length > 0);
        },

        showFilterRow() {
            let show_row = false;

            this.columns.some((column,key) => {
                if (has(column, "filter")) {
                    show_row = true;
                    return true;
                }
            })
            return show_row;
        },

        showPaginationRow() {
            let show_pagination_row = false;

            if (this.card_mode == false && (this.pagination == true || this.pagination_info == true || this.selected_rows_info == true)) {
                show_pagination_row = true;
            }

            return show_pagination_row;
        },

        currentPageSelectionCount() {
            let result = [];
            if (this.server_mode && !this.hasUniqueId) {
                result = intersectionWith(this.vbt_rows, this.selected_items, isEqual);
            } else {
                result = intersectionBy(this.vbt_rows, this.selected_items, this.uniqueId);
            }
            return result.length;
        },
        tableClasses() {
            let classes = "";
            if (typeof this.classes.table == "string") {
                return this.classes.table;
            } else if (typeof this.classes.table == "object") {
                Object.entries(this.classes.table).forEach(([key, value]) => {
                    if (typeof value == "boolean" && value) {
                        classes += key;
                    } else if (typeof value == "function") {
                        let truth = value(this.rows);
                        if (typeof truth == "boolean" && truth) {
                            classes += " ";
                            classes += key;
                        }
                    }
                });
            }
            return classes;
        },
        tableWrapperClasses() {
          let classes = "";
          let defaultClasses = this.isResponsive ? "table-responsive" : "";

            if (!this.classes.tableWrapper && this.classes.tableWrapper != "") {
                return defaultClasses;
            }

            return (typeof this.classes.tableWrapper == "string") ? this.classes.tableWrapper : defaultClasses;
        },

        isSelectable() {
            return (this.checkbox_rows || this.rows_selectable);
        },

        updateGlobalSearch() {
            return debounce(this.updateGlobalSearchHandler, this.global_search.searchDebounceRate);
        }
    },
    watch: {
        "query.filters": {
            handler: function (after, before) {
                if (!this.server_mode) {
                    this.filter(!this.preservePageOnDataChange);
                }
            },
            deep: true
        },
        "query.sort": {
            handler: function (after, before) {
                if (!this.server_mode) {
                    this.sort();
                }
            },
            deep: true
        },
        "query.global_search": {
            handler: function (newVal, oldVal) {
                if (!this.server_mode) {
                    this.filter(!this.preservePageOnDataChange);
                }
            }
        },
        "query": {
            handler: function (newVal, oldVal) {
                if (this.server_mode) {
                    this.emitQueryParams();
                }
            },
            deep: true
        },
        per_page: {
            handler: function (newVal, oldVal) {
                if (!this.server_mode) {
                    let doPaginateFilter = (this.page == 1);
                    if (!this.preservePageOnDataChange) this.page = 1;
                    if (doPaginateFilter) {
                        this.paginateFilter();
                    }
                } else {
                    this.emitQueryParams();
                }
            }
        },
        pagination: {
            handler: function (newVal, oldVal) {
                if (!this.server_mode) {
                    this.paginateFilter();
                } else {
                    this.emitQueryParams();
                }
            }
        },
        rows: {
            handler: function (newVal, oldVal) {

                this.vbt_rows = cloneDeep(this.rows);

                if (!this.server_mode) {
                    // check if user mentioned unique id for a column, if not set unique id for all items
                    let self = this;
                    this.original_rows = map(this.vbt_rows, function (element, index) {
                        let extra = {};
                        if (!self.hasUniqueId) {
                            extra.vbt_id = index + 1;
                        }
                        return extend({}, element, extra);
                    });
                    this.filter(!this.preservePageOnDataChange,!this.isFirstTime);
                } else {
                    if (this.preservePageOnDataChange) {
                        let predictedTotalPage = Math.ceil(this.rowCount/this.per_page);
                        if (predictedTotalPage != 0) {
                            this.page = (this.page <= predictedTotalPage) ? this.page : predictedTotalPage;
                        } else {
                            this.page = 1;
                        }
                    }
                }

                this.isFirstTime = false;
            },
            deep: true
        },
        customFilters: {
            handler: function (newVal, oldVal) {

                if (!this.server_mode) {
                    newVal.forEach(customFilter => {
                        if (customFilter.name) {
                            let index = this.query.filters.findIndex( filter => filter.name === customFilter.name );
                            if (index == -1) {
                                this.query.filters.push(customFilter);
                            } else {
                                this.query.filters[index].text = customFilter.text;
                            }
                        }
                    });
                }

            },
            deep: true
        },
        columns: {
            handler: function (newVal, oldVal) {

                this.vbt_columns = cloneDeep(this.columns);

                this.vbt_columns = map(this.vbt_columns, function (element, index) {
                    let extra = {};
                    extra.vbt_col_id = index + 1;
                    return extend({}, element, extra);
                });

                this.initFilterQueries();
            },
            deep: true
        },
        config: {
            handler: function (newVal, oldVal) {
                this.initConfig();
            },
            deep: true
        },

        vbt_rows: {
            handler: function (newVal, oldVal) {
                // resetting the shift mode
                this.lastSelectedItemIndex = null;

                if (this.selected_items.length == 0) {
                    // this.$eventHub.emit('unselect-select-all-items-checkbox');
                    this.allRowsSelected = false;
                    return;
                }

                let difference = [];

                if (this.server_mode && !this.hasUniqueId) {
                    difference = differenceWith(newVal, this.selected_items, isEqual);
                } else {
                    difference = differenceBy(newVal, this.selected_items, this.uniqueId);
                }

                if (difference.length == 0) {
                    // this.$eventHub.emit('select-select-all-items-checkbox');
                    this.allRowsSelected = true;
                } else {
                    this.allRowsSelected = false;
                    // this.$eventHub.emit('unselect-select-all-items-checkbox');
                }

            },
            deep: true
        },

        page(newVal, oldVal) {
            if (!this.server_mode) {
                this.paginateFilter();
            } else {
                this.emitQueryParams(newVal);
            }
        },
        'config.multi_column_sort': {
            handler : function(newVal,oldVal) {
                this.resetSort();
            }
        },
        'currentLang': {
            handler : function() {
                this.$forceUpdate();
            } 
        }
    }
};
</script>

<style scoped>
    .vbt-select-all-checkbox {
        margin-bottom: 20px;
    }

    .vbt-sort-cursor {
        cursor: pointer;
    }
    .custom-control-label {
        vertical-align: top;
    }
    .vbt-column-header {
        -webkit-user-select: none;  /* Chrome all / Safari all */
        -moz-user-select: none;     /* Firefox all */
        -ms-user-select: none;      /* IE 10+ */
        user-select: none;          /* Likely future */
    }
    .vbt-global-search-clear {
        cursor: pointer;
    }
    input[type="search"] {
    -webkit-appearance: searchfield;
    }

    /* Bootstrap 4 text input with clear icon on the right */

    .has-clear-right {
        position: relative;
    }

    .has-clear-right .form-control {
        padding-right: 2.375rem;
    }

    .has-clear-right .form-control-feedback {
        position: absolute;
        top: 0;
        right: 0;
        z-index: 2;
        display: block;
        width: 2.375rem;
        height: 2.375rem;
        line-height: 2.375rem;
        text-align: center;
        font-weight: normal;
    }

    .has-clear-right .form-control-feedback:hover {
        color: red;
    }

  /* Absolute Center Spinner */
.loading {
  position: fixed;
  z-index: 999;
  height: 2em;
  width: 2em;
  overflow: visible;
  margin: auto;
  top: 0;
  left: 0;
  bottom: 0;
  right: 0;
}

/* Transparent Overlay */
.loading:before {
  content: '';
  display: block;
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0,0,0,0.3);
}

/* :not(:required) hides these rules from IE9 and below */
.loading:not(:required) {
  /* hide "loading..." text */
  font: 0/0 a;
  color: transparent;
  text-shadow: none;
  background-color: transparent;
  border: 0;
}

.loading:not(:required):after {
  content: '';
  display: block;
  font-size: 10px;
  width: 1em;
  height: 1em;
  margin-top: -0.5em;
  -webkit-animation: spinner 1500ms infinite linear;
  -moz-animation: spinner 1500ms infinite linear;
  -ms-animation: spinner 1500ms infinite linear;
  -o-animation: spinner 1500ms infinite linear;
  animation: spinner 1500ms infinite linear;
  border-radius: 0.5em;
  -webkit-box-shadow: rgba(0, 0, 0, 0.75) 1.5em 0 0 0, rgba(0, 0, 0, 0.75) 1.1em 1.1em 0 0, rgba(0, 0, 0, 0.75) 0 1.5em 0 0, rgba(0, 0, 0, 0.75) -1.1em 1.1em 0 0, rgba(0, 0, 0, 0.5) -1.5em 0 0 0, rgba(0, 0, 0, 0.5) -1.1em -1.1em 0 0, rgba(0, 0, 0, 0.75) 0 -1.5em 0 0, rgba(0, 0, 0, 0.75) 1.1em -1.1em 0 0;
  box-shadow: rgba(0, 0, 0, 0.75) 1.5em 0 0 0, rgba(0, 0, 0, 0.75) 1.1em 1.1em 0 0, rgba(0, 0, 0, 0.75) 0 1.5em 0 0, rgba(0, 0, 0, 0.75) -1.1em 1.1em 0 0, rgba(0, 0, 0, 0.75) -1.5em 0 0 0, rgba(0, 0, 0, 0.75) -1.1em -1.1em 0 0, rgba(0, 0, 0, 0.75) 0 -1.5em 0 0, rgba(0, 0, 0, 0.75) 1.1em -1.1em 0 0;
}

/* Animation */

@-webkit-keyframes spinner {
  0% {
    -webkit-transform: rotate(0deg);
    -moz-transform: rotate(0deg);
    -ms-transform: rotate(0deg);
    -o-transform: rotate(0deg);
    transform: rotate(0deg);
  }
  100% {
    -webkit-transform: rotate(360deg);
    -moz-transform: rotate(360deg);
    -ms-transform: rotate(360deg);
    -o-transform: rotate(360deg);
    transform: rotate(360deg);
  }
}
@-moz-keyframes spinner {
  0% {
    -webkit-transform: rotate(0deg);
    -moz-transform: rotate(0deg);
    -ms-transform: rotate(0deg);
    -o-transform: rotate(0deg);
    transform: rotate(0deg);
  }
  100% {
    -webkit-transform: rotate(360deg);
    -moz-transform: rotate(360deg);
    -ms-transform: rotate(360deg);
    -o-transform: rotate(360deg);
    transform: rotate(360deg);
  }
}
@-o-keyframes spinner {
  0% {
    -webkit-transform: rotate(0deg);
    -moz-transform: rotate(0deg);
    -ms-transform: rotate(0deg);
    -o-transform: rotate(0deg);
    transform: rotate(0deg);
  }
  100% {
    -webkit-transform: rotate(360deg);
    -moz-transform: rotate(360deg);
    -ms-transform: rotate(360deg);
    -o-transform: rotate(360deg);
    transform: rotate(360deg);
  }
}
@keyframes spinner {
  0% {
    -webkit-transform: rotate(0deg);
    -moz-transform: rotate(0deg);
    -ms-transform: rotate(0deg);
    -o-transform: rotate(0deg);
    transform: rotate(0deg);
  }
  100% {
    -webkit-transform: rotate(360deg);
    -moz-transform: rotate(360deg);
    -ms-transform: rotate(360deg);
    -o-transform: rotate(360deg);
    transform: rotate(360deg);
  }
}
</style>


// workflow - server
// get data(payload)
// clone to origin_rows
// do filter
// do global search
// do sort
// do paginate
