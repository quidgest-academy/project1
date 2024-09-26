<template>
	<teleport :to="`#q-modal-file-properties-${controlId}-body`">
		<div class="q-document__properties">
			<q-text-field
				:model-value="fileProperties.name"
				readonly
				size="block"
				:label="texts.nameLabel" />

			<q-text-field
				:model-value="fileProperties.size"
				readonly
				size="block"
				:label="texts.sizeLabel" />

			<q-text-field
				:model-value="fileProperties.fileType"
				readonly
				size="block"
				:label="texts.extensionLabel" />

			<q-text-field
				:model-value="fileProperties.author"
				readonly
				size="block"
				:label="texts.authorLabel" />

			<q-text-field
				:model-value="fileProperties.createdDate"
				readonly
				size="block"
				:label="texts.createdDateLabel" />

			<q-text-field
				:model-value="fileProperties.version"
				readonly
				size="block"
				:label="texts.currentVersionLabel" />

			<q-text-field
				:model-value="fileProperties.editor"
				readonly
				size="block"
				:label="texts.editedByLabel" />
		</div>
	</teleport>

	<teleport :to="`#q-modal-file-properties-${controlId}-footer`">
		<div class="actions">
			<q-button
				b-style="primary"
				:label="texts.okLabel"
				:title="texts.okLabel"
				@click="$emit('hide-popup')">
				<q-icon icon="ok" />
			</q-button>
		</div>
	</teleport>
</template>

<script>
	export default {
		name: 'QDocumentProperties',

		emits: {
			'hide-popup': () => true
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique ID for the control.
			 */
			controlId: String,

			/**
			 * Necessary strings to be used in labels and buttons.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * The properties of the document.
			 */
			fileProperties: {
				type: Object,
				default: () => ({})
			}
		},

		expose: []
	}
</script>
