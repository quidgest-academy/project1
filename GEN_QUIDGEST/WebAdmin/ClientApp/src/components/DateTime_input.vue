<template>
    <div class="i-date-picker">
      <div class="d-flex" v-if="label">
        <label class="i-text__label i-text" :for="id">{{ label }}</label>
      </div>
      <div class="i-input-group date">
		<Datepicker :id="id" :modelValue="modelValue" @update:modelValue="updateValue" :readonly="isReadOnly" data-ref="cur_elem"></Datepicker>
        <!-- 
          <div :id="id" class="datetimepicker-inline i-input-group__field b-icon-text input-medium" v-if="config.inline" data-ref="cur_elem"></div>
          <input type="text" :id="id" class="i-input-group__field b-icon-text input-medium" :readonly="isReadOnly" data-ref="cur_elem" v-else>
          <div class="i-input-group--right i-date-picker__button">
              <button class="i-date-picker__button--secondary" type="button" @click="pluginClick">
                <span class="glyphicons glyphicons-calendar i-input-group__tag-icon"></span>
              </button>
          </div>
		-->
      </div>
    </div>
</template>

<script>
    //import 'pc-bootstrap4-datetimepicker';
    //import 'pc-bootstrap4-datetimepicker/build/css/bootstrap-datetimepicker.css';
    import moment from 'moment';
    // https://www.npmjs.com/package/vue-bootstrap-datetimepicker
    // http://eonasdan.github.io/bootstrap-datetimepicker/Options/

	import Datepicker from '@vuepic/vue-datepicker';
	import '@vuepic/vue-datepicker/dist/main.css';

    export default {
        name: 'datetime-input',
		components: { Datepicker },
        emits: ['update:modelValue', 'dp-hide', 'dp-show', 'dp-change', 'dp-error', 'dp-update'],
        props: {
            modelValue: {
                default: null,
                required: true,
                validator(modelValue) {
                    return modelValue === null || modelValue instanceof Date || typeof modelValue === 'string' || modelValue instanceof String || modelValue instanceof moment
                }
            },
            label: String,
            isReadOnly: Boolean
        },
        data: function () {
            return {
                id: null,
                dp: null,
                // jQuery DOM
                elem: null,
                // http://eonasdan.github.io/bootstrap-datetimepicker/Options/
                config: {
                    showClear: true,
                    icons: {
                        time: 'e-icon glyphicons glyphicons-clock',
                        date: 'e-icon glyphicons glyphicons-calendar',
                        up: 'e-icon glyphicons glyphicons-arrow-up',
                        down: 'e-icon glyphicons glyphicons-arrow-down',
                        previous: 'e-icon glyphicons glyphicons-chevron-left',
                        next: 'e-icon glyphicons glyphicons-chevron-right',
                        today: 'e-icon glyphicons glyphicons-important-day',
                        clear: 'e-icon glyphicons glyphicons-bin',
                        close: 'e-icon glyphicons glyphicons-remove-sign'
                    }
                },
                events: ['hide', 'show', 'change', 'error', 'update']
            }
        },
        mounted: function () {
            var vm = this;//, comp = $(vm.$el);
            vm.id = 'input_t_' + vm._.uid;

            //console.warn("Date-input Mounted");

            // Return early if date-picker is already loaded
            /* istanbul ignore if */
			/*
            if (this.dp) return;
            // Handle wrapped input
            this.elem = $('[data-ref="cur_elem"]', this.$el);
            // Init date-picker
            this.elem.datetimepicker(this.config);
            // Store data control
            this.dp = this.elem.data('DateTimePicker');
            // Set initial value
            this.dp.date(this.modelValue);
            // Watch for changes
            this.elem.on('dp.change', this.onChange);
            // Register remaining events
            this.registerEvents();
			*/
        },
        watch: {
            /**
             * Listen to change from outside of component and update DOM
             *
             * @param newValue
             */
            modelValue(newValue) {
                this.dp && this.dp.date(newValue || null)
            },

            /**
             * Watch for any change in options and set them
             *
             * @param newConfig Object
             */
            config: {
                deep: true,
                handler(newConfig) {
                    this.dp && this.dp.options(newConfig);
                }
            }
        },
        methods: {
            /**
             * Update v-model upon change triggered by date-picker itself
             *
             * @param event
             */
            onChange(event) {
                let formattedDate = event.date ? event.date.format(this.dp.format()) : null;
                this.$emit('update:modelValue', formattedDate);
            },

            updateValue(newValue) {
                this.$emit('update:modelValue', newValue);
            },

            /**
             * Emit all available events
             */
            registerEvents() {
			/*
                this.events.forEach((name) => {
                    this.elem.on(`dp.${name}`, (...args) => {
                        this.$emit(`dp-${name}`, ...args);
                    });
                })
				*/
            },
            pluginClick() {
                if (!this.isReadOnly && this.dp) {
                    this.dp.toggle();
                }
            }
        },
        /**
         * Free up memory
         */
        beforeUnmount() {
            /* istanbul ignore else */
            if (this.dp) {
                this.dp.destroy();
                this.dp = null;
                this.elem = null;
            }
        }
    };
</script>


