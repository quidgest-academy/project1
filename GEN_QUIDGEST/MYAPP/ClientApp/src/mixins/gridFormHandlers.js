import { computed } from 'vue'

import _find from 'lodash-es/find'

import hardcodedTexts from '@/hardcodedTexts.js'
import formControlClass from '@/mixins/formControl.js'
import FormHandlers from '@/mixins/formHandlers.js'

/*****************************************************************
 * This mixin defines methods to be reused by editable table     *
 * list forms.                                                   *
 *****************************************************************/
export default {
	emits: {
		'mark-for-deletion': () => true,
		'undo-deletion': () => true,
		'toggle-errors': () => true
	},

	mixins: [
		FormHandlers
	],

	props: {
		/**
		 * The initial state of the editable table list row.
		 */
		initialState: {
			type: String,
			default: ''
		},

		/**
		 * Array containing column definitions.
		 */
		columns: {
			type: Array,
			required: true
		},

		/**
		 * Is deleted state mode
		 * it was necessary because when you navigate to different form and come back we need to now if the row was deleted before. 
		 * if it was, the state will be "Deleted" and the undo button will appear. 
		 */
		isDeletedState: {
			type: Boolean,
			default: false
		},
	},

	data()
	{
		return {
			currentRouteParams: {},

			formButtons: new formControlClass.FormControlButtons(),

			texts: {
				delete: computed(() => this.Resources[hardcodedTexts.delete]),
				remove: computed(() => this.Resources[hardcodedTexts.remove]),
				restore: computed(() => this.Resources[hardcodedTexts.restore]),
				messages: computed(() => this.Resources[hardcodedTexts.messages]),
			},

			markedForDeletion: false,

			expandIcon: 'expand',
			expandSVG: 'expand',
			collapseSVG: 'collapse',
		}
	},

	computed: {
		/**
		 * The state of the row.
		 */
		state()
		{
			if (this.markedForDeletion || this.isDeletedState)
				return 'DELETED'
			else if (this.nestedModel.hasServerErrorMessages())
				return 'ERRORS'
			else if (this.nestedModel.hasServerWarningMessages())
				return 'WARNINGS'
			else if (this.nestedModel.serverInfoMessages?.length > 0)
				return 'INFO'
			else if (this.initialState === 'NEW')
				return this.nestedModel.isDirty ? 'NEW' : 'NEW--EMPTY'
			return this.nestedModel.isDirty ? 'EDITED' : ''
		},

		/**
		 * The class associated to the state of the row.
		 */
		rowClass()
		{
			return `grid-table-row${this.state !== '' ? '__' + this.state.toLowerCase() : ''}`
		},

		/**
		 * The icon associated to the state of the row.
		 */
		rowStateIcon()
		{
			switch (this.state)
			{
				case 'NEW':
					return 'add-outline'
				case 'NEW--EMPTY':
					return 'add'
				case 'EDITED':
					return 'pencil'
				case 'ERRORS':
				case 'WARNINGS':
				case 'INFO':
					return 'exclamation-sign'
				case 'DELETED':
					return 'delete'
				default:
					return ''
			}
		},

		/**
		 * The badge style to be applied based on the messages shown
		 */
		badgeColor() {
			const colorMap = {
				'ERRORS': 'danger',
				'WARNINGS': 'warning'
			}

			return colorMap[this.state] || 'info'
		},

		/**
		 * Whether to show the "delete" button or not.
		 * This button should be visible for all pre-existing rows.
		 */
		showDeleteBtn()
		{
			return this.mode === 'EDIT'
				&& this.permissions.canDelete
				&& this.initialState === ''
		},

		/**
		 * Whether to show the "remove" button or not.
		 * This button should be visible for new rows.
		 */
		showRemoveBtn()
		{
			return this.initialState === 'NEW'
				&& this.nestedModel.isDirty
		},

		/**
		 * Whether to show the "undo" button or not.
		 * This button should be visible for rows that are
		 * marked to be deleted.
		 */
		showUndoBtn()
		{
			return this.state === 'DELETED'
		},

		/**
		 * Indicates the number of messages
		 */
		numMessages() {
			const allMessages = []
			return allMessages.concat(this.nestedModel?.serverErrorMessages,
				this.nestedModel?.serverWarningMessages,
				this.nestedModel?.serverInfoMessages ?? []).length
		},

		/**
		 * Indicates if there are any messages to show on the row
		 */
		hasMessages() {
			return this.numMessages > 0
		},
	},

	methods: {
		/**
		 * Marks this row for deletion when the main form is saved.
		 */
		markForDeletion()
		{
			this.markedForDeletion = true
			this.emitEvent('mark-for-deletion')
		},

		/**
		 * Undoes the "Mark for deletion" action.
		 */
		undoMarkForDeletion()
		{
			this.markedForDeletion = false
			this.emitEvent('undo-deletion')
		},

		/**
		 * Show the list of errors, warnings and information about this model
		 */
		toggleErrors()
		{
			this.expandIcon = this.expandIcon === this.expandSVG ? this.collapseSVG : this.expandSVG
			this.emitEvent('toggle-errors')
		},

		/**
		 * Determine if column is visible
		 * @param {string} area The column table 
		 * @param {string} field The column field
		 * @returns {Boolean} If column is visible
		 */
		canShowColumn(area, field)
		{
			const column = _find(this.columns, { area, field })
			return column?.visibility ?? false
		}
	}
}
