<template>
	<div class="q-cards-carousel__container">
		<div
			ref="container"
			class="row q-cards-carousel"
			@scroll.passive="onScroll">
			<template v-if="loading">
				<div
					v-for="skeleton in 10"
					:key="skeleton"
					class="col">
					<q-card
						loading
						v-bind="cardConfig">
						<template #title></template>
						<template #subtitle></template>
						<template #text></template>
						<template #image></template>
					</q-card>
				</div>
			</template>
			<slot v-else />
		</div>

		<div
			v-if="showPrevBtn"
			class="q-cards-carousel__btn q-cards-carousel__btn-prev"
			@click.stop="prev"
			role="button">
			<div class="q-cards-carousel__btn-icon">
				<q-icon icon="step-back" />
			</div>
		</div>

		<div
			v-if="showNextBtn"
			class="q-cards-carousel__btn q-cards-carousel__btn-next"
			@click.stop="next"
			role="button">
			<div class="q-cards-carousel__btn-icon">
				<q-icon icon="step-forward" />
			</div>
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QCardCarousel',

		inheritAttrs: false,

		props: {
			/**
			 * The configuration of a card.
			 */
			cardConfig: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Whether or not content is loading.
			 */
			loading: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				delta: 2.5,
				showPrevBtn: false,
				showNextBtn: false
			}
		},

		mounted()
		{
			this.updateControls()
		},

		beforeUnmount()
		{
			clearTimeout(this.updateId)
		},

		methods: {
			hasPrev()
			{
				const container = this.$refs.container

				if (container.scrollLeft === 0)
					return false

				const firstCard = this.$refs.container.children[0]
				const containerVWLeft = container.getBoundingClientRect().left
				const firstCardLeft = firstCard?.getBoundingClientRect()?.left ?? 0

				return Math.abs(containerVWLeft - firstCardLeft) >= this.delta
			},

			hasNext()
			{
				const container = this.$refs.container

				return (
					container.scrollWidth >
					container.scrollLeft + container.clientWidth + this.delta
				)
			},

			prev()
			{
				const container = this.$refs.container
				const left = container.getBoundingClientRect().left
				const x = left - container.clientWidth - this.delta
				const card = this.findPrevCard(x)

				if (card)
				{
					const offset = card.getBoundingClientRect().left - left
					this.scrollTo(container.scrollLeft + offset)
					return
				}

				this.scrollTo(container.scrollLeft - container.clientWidth)
			},

			next()
			{
				const container = this.$refs.container
				const left = container.getBoundingClientRect().left
				const x = left + container.clientWidth + this.delta
				const card = this.findNextCard(x)

				if (card)
				{
					const offset = card.getBoundingClientRect().left - left
					if (offset > this.delta)
					{
						this.scrollTo(container.scrollLeft + offset)
						return
					}
				}

				this.scrollTo(container.scrollLeft + container.clientWidth)
			},

			findPrevCard(x)
			{
				const cards = this.$refs.container.children

				for (let i = 0; i < cards.length; i++)
				{
					const card = cards[i].getBoundingClientRect()

					if (card.left <= x && x <= card.right)
						return cards[i]
					else if (x <= card.left)
						return cards[i]
				}
			},

			findNextCard(x)
			{
				const cards = this.$refs.container.children

				for (let i = 0; i < cards.length; i++)
				{
					const card = cards[i].getBoundingClientRect()

					if (card.right <= x)
						continue // x is after this card
					else if (card.left <= x)
						return cards[i] // card.left <= x <= card.right

					// If x is somewhere after the previous card,
					// but before this card,
					// then this is the "next" card
					if (x <= card.left)
						return cards[i]
				}
			},

			scrollTo(offset)
			{
				this.$refs.container.scrollTo({ left: offset, behavior: 'smooth' })
			},

			updateControls()
			{
				this.$nextTick().then(() => {
					this.showPrevBtn = this.hasPrev()
					this.showNextBtn = this.hasNext()
				})
			},

			onScroll()
			{
				clearTimeout(this.updateId)
				this.updateId = setTimeout(this.updateControls, 100)
			}
		}
	}
</script>
