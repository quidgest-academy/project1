<template>
	<h3>Date formats</h3>

	<base-input-structure
		id="CTRL_1"
		label="Hours">
		<q-date-time-picker
			id="CTRL_1"
			format="time"
			v-model="mockDates.date1"
			@update:model-value="updateDate('date1', $event)" />
	</base-input-structure>

	<base-input-structure
		id="CTRL_2"
		label="Date (default format: dd/MM/yyyy)">
		<q-date-time-picker
			id="CTRL_2"
			format="date"
			v-model="mockDates.date2"
			@update:model-value="updateDate('date2', $event)" />
	</base-input-structure>

	<base-input-structure
		id="CTRL_3"
		label="Date (format: dd-MM-yyyy)">
		<q-date-time-picker
			id="CTRL_3"
			format="date"
			v-model="mockDates.date3"
			:date-format="{ Date: 'dd-MM-yyyy' }"
			@update:model-value="updateDate('date3', $event)" />
	</base-input-structure>

	<base-input-structure
		id="CTRL_4"
		label="Date (format: MM/dd/yyyy)">
		<q-date-time-picker
			id="CTRL_4"
			format="date"
			v-model="mockDates.date4"
			:date-format="{ Date: 'MM/dd/yyyy' }"
			@update:model-value="updateDate('date4', $event)" />
	</base-input-structure>

	<base-input-structure
		id="CTRL_5"
		label="Date (format: yyyy/MM/dd)">
		<q-date-time-picker
			id="CTRL_5"
			format="date"
			v-model="mockDates.date5"
			:date-format="{ Date: 'yyyy/MM/dd' }"
			@update:model-value="updateDate('date5', $event)" />
	</base-input-structure>

	<base-input-structure
		id="CTRL_6"
		label="Date and time (default format: dd/MM/yyyy HH:mm)">
		<q-date-time-picker
			id="CTRL_6"
			format="dateTime"
			v-model="mockDates.date6"
			@update:model-value="updateDate('date6', $event)" />
	</base-input-structure>

	<base-input-structure
		id="CTRL_7"
		label="Date and time (format: dd-MM-yyyy HH:mm)">
		<q-date-time-picker
			id="CTRL_7"
			format="date"
			v-model="mockDates.date7"
			:date-format="{ Date: 'dd-MM-yyyy HH:mm' }"
			@update:model-value="updateDate('date7', $event)" />
	</base-input-structure>

	<base-input-structure
		id="CTRL_8"
		label="Date, hours and seconds (default format: dd/MM/yyyy HH:mm:ss)">
		<q-date-time-picker
			id="CTRL_8"
			format="dateTimeSeconds"
			v-model="mockDates.date8"
			@update:model-value="updateDate('date8', $event)" />
	</base-input-structure>

	<base-input-structure
		id="CTRL_9"
		label="Date, hours and seconds (format: dd-MM-yyyy HH:mm:ss)">
		<q-date-time-picker
			id="CTRL_9"
			format="dateTimeSeconds"
			v-model="mockDates.date9"
			:date-format="{ DateTimeSeconds: 'dd-MM-yyyy HH:mm:ss' }"
			@update:model-value="updateDate('date9', $event)" />
	</base-input-structure>

	<base-input-structure
		id="CTRL_10"
		label="Date and time (disabled: true)">
		<q-date-time-picker
			id="CTRL_10"
			format="dateTime"
			v-model="mockDates.date10"
			disabled
			@update:model-value="updateDate('date10', $event)" />
	</base-input-structure>

	<base-input-structure
		id="CTRL_11"
		label="Date and time (readonly: true)">
		<q-date-time-picker
			id="CTRL_11"
			format="dateTime"
			v-model="mockDates.date11"
			readonly
			@update:model-value="updateDate('date11', $event)" />
	</base-input-structure>

	<base-input-structure
		id="CTRL_12"
		label="Date and time with filled value (readonly: true)">
		<q-date-time-picker
			id="CTRL_12"
			format="dateTime"
			v-model="mockDates.date12"
			readonly
			:date-format="{ Date: 'dd/MM/yyyy' }"
			@update:model-value="updateDate('date12', $event)" />
	</base-input-structure>
</template>

<script>
	export default {
		name: 'QDatePickerContainer',

		docsfile: './docs/inputs/DateInput.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				mockDates: {
					date1: '',
					date2: '',
					date3: '',
					date4: '',
					date5: '',
					date6: '',
					date7: '',
					date8: '',
					date9: '',
					date10: '',
					date11: '',
					date12: '12/04/2022'
				}
			}
		},

		methods: {
			updateDate(dateCtrl, newDate)
			{
				this.mockDates[dateCtrl] = newDate
			}
		}
	}
</script>
