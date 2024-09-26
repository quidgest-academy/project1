import screenfull from 'screenfull'

export function addFullScreenMode(leaflet)
{
	leaflet.Control.FullScreen = leaflet.Control.extend({
		options: {
			position: 'topleft',
			title: 'Full Screen',
			titleCancel: 'Exit Full Screen',
			forceSeparateButton: false,
			forcePseudoFullscreen: false,
			fullscreenElement: false
		},

		_screenfull: screenfull,

		onAdd: function (map) {
			var className = 'leaflet-control-zoom-fullscreen', container, content = ''

			if (map.zoomControl && !this.options.forceSeparateButton) {
				container = map.zoomControl._container
			} else {
				container = leaflet.DomUtil.create('div', 'leaflet-bar')
			}

			if (this.options.content) {
				content = this.options.content
			} else {
				className += ' fullscreen-icon'
			}

			this._createButton(this.options.title, className, content, container, this.toggleFullScreen, this)
			this._map.fullscreenControl = this

			this._map.on('enterFullscreen exitFullscreen', this._toggleState, this)

			return container
		},

		onRemove: function () {
			leaflet.DomEvent
				.off(this.link, 'click', leaflet.DomEvent.stop)
				.off(this.link, 'click', this.toggleFullScreen, this)

			if (this._screenfull.isEnabled) {
				leaflet.DomEvent
					.off(this._container, this._screenfull.raw.fullscreenchange, leaflet.DomEvent.stop)
					.off(this._container, this._screenfull.raw.fullscreenchange, this._handleFullscreenChange, this)

				leaflet.DomEvent
					.off(document, this._screenfull.raw.fullscreenchange, leaflet.DomEvent.stop)
					.off(document, this._screenfull.raw.fullscreenchange, this._handleFullscreenChange, this)
			}
		},

		_createButton: function (title, className, content, container, fn, context) {
			this.link = leaflet.DomUtil.create('a', className, container)
			this.link.href = '#'
			this.link.title = title
			this.link.innerHTML = content

			this.link.setAttribute('role', 'button')
			this.link.setAttribute('aria-label', title)

			leaflet.DomEvent.disableClickPropagation(container)

			leaflet.DomEvent
				.on(this.link, 'click', leaflet.DomEvent.stop)
				.on(this.link, 'click', fn, context)

			if (this._screenfull.isEnabled) {
				leaflet.DomEvent
					.on(container, this._screenfull.raw.fullscreenchange, leaflet.DomEvent.stop)
					.on(container, this._screenfull.raw.fullscreenchange, this._handleFullscreenChange, context)

				leaflet.DomEvent
					.on(document, this._screenfull.raw.fullscreenchange, leaflet.DomEvent.stop)
					.on(document, this._screenfull.raw.fullscreenchange, this._handleFullscreenChange, context)
			}

			return this.link
		},

		toggleFullScreen: function () {
			var map = this._map
			map._exitFired = false
			if (map._isFullscreen) {
				if (this._screenfull.isEnabled && !this.options.forcePseudoFullscreen) {
					this._screenfull.exit()
				} else {
					leaflet.DomUtil.removeClass(this.options.fullscreenElement ? this.options.fullscreenElement : map._container, 'leaflet-pseudo-fullscreen')
					map.invalidateSize()
				}
				map.fire('exitFullscreen')
				map._exitFired = true
				map._isFullscreen = false
			}
			else {
				if (this._screenfull.isEnabled && !this.options.forcePseudoFullscreen) {
					this._screenfull.request(this.options.fullscreenElement ? this.options.fullscreenElement : map._container)
				} else {
					leaflet.DomUtil.addClass(this.options.fullscreenElement ? this.options.fullscreenElement : map._container, 'leaflet-pseudo-fullscreen')
					map.invalidateSize()
				}
				map.fire('enterFullscreen')
				map._isFullscreen = true
			}
		},

		_toggleState: function () {
			this.link.title = this._map._isFullscreen ? this.options.title : this.options.titleCancel
			this._map._isFullscreen ? leaflet.DomUtil.removeClass(this.link, 'leaflet-fullscreen-on') : leaflet.DomUtil.addClass(this.link, 'leaflet-fullscreen-on')
		},

		_handleFullscreenChange: function () {
			var map = this._map
			map.invalidateSize()
			if (!this._screenfull.isFullscreen && !map._exitFired) {
				map.fire('exitFullscreen')
				map._exitFired = true
				map._isFullscreen = false
			}
		}
	})

	leaflet.control.fullscreen = function (options) {
		return new leaflet.Control.FullScreen(options)
	}

	return leaflet
}

export default {
	addFullScreenMode
}