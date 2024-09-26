<template>
    <div class="col-auto search" style="display: block;" v-if="visibility">
        <div class="form-group has-clear-right" :class="classes">
            <span v-if="showClearButton" class="form-control-feedback vbt-global-search-clear" @click="clearGlobalSearch">
                <slot name="global-search-clear-icon">
                    &#x24E7;
                </slot>
            </span>
            <div class="i-input-group input-xxlarge">
                <template v-if="searchOnPressEnter">
                    <input ref="globalSearch" type="text" class="i-input-group__field input-xxlarge" :placeholder="placeholder" @keyup.enter="$emit('emitSearch', $refs.globalSearch.value)" />
                </template>
                <template v-else>
                    <input ref="globalSearch" type="text" class="i-input-group__field input-xxlarge" :placeholder="placeholder" />
                </template>
                <div class="i-input-group--right">
                    <!-- refresh & reset button starts here -->
                    <button v-if="showRefreshButton" type="button" class="b-icon--secondary i-input-group__button--secondary listSearchButton" @click="$emit('emitSearch', $refs.globalSearch.value)">
                        <slot name="refresh-button-text">
                            <i class="glyphicons glyphicons-search i-input-group__tag-icon i-input-group__button-icon"></i>
                        </slot>
                    </button>
                    <button type="button" v-if="showResetButton" class="b-icon--secondary i-input-group__button--secondary" @click="resetQuery">
                        <slot name="reset-button-text">
                            <span class="glyphicons glyphicons-remove-sign i-input-group__tag-icon i-input-group__button-icon"></span>
                        </slot>
                    </button>
                    <!-- refresh & reset button ends here -->
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        name: "Search",
        props: {
            initPlaceholder: {
                type: String,
                default: ""
            },
            initClasses: {
                type: String,
                default: ""
            },
            initVisibility: {
                type: Boolean,
                default: true
            },
            initCaseSensitive: {
                type: Boolean,
                default: false
            },
            initShowRefreshButton: {
                type: Boolean,
                default: true
            },
            initShowResetButton: {
                type: Boolean,
                default: true
            },
            initShowClearButton: {
                type: Boolean,
                default: false
            },
            initSearchOnPressEnter: {
                type: Boolean,
                default: false
            },
            initSearchDebounceRate: {
                type: Number,
                default: 60
            }
        },
        data: function () {
            return {
                placeholder: this.initPlaceholder,
                classes: this.initClasses,
                visibility: this.initVisibility,
                caseSensitive: this.initCaseSensitive,
                showRefreshButton: this.initShowRefreshButton,
                showResetButton: this.initShowResetButton,
                showClearButton: this.initShowClearButton,
                searchOnPressEnter: this.initSearchOnPressEnter,
                searchDebounceRate: this.initSearchDebounceRate
            }
        },
        methods: {

            clearGlobalSearch() {
                this.$refs.global_search.value = "";
                this.$emit('clearGlobalSearch');
            },

            resetQuery() {
                this.$refs.globalSearch.value = "";
                this.$emit('resetQuery');
                //this.$eventHub.$emit('reset-query');
            }

        },
        emits: ['clearGlobalSearch', 'updateGlobalSearchHandler', 'updateGlobalSearch', 'emitSearch', 'resetQuery']
    };
</script>

<style scoped>
    .vbt-global-search-clear {
        cursor: pointer;
    }
</style>