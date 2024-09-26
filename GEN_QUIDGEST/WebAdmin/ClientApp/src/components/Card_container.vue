<template>
  <div :class="style_class">
    <div class="c-card__header" v-bind:class="{'c-card__header--collapse': isCollapse}" v-on="isCollapse ? { click: () => $refs.collapseButton.click() } : {}">
      <button v-if="isCollapse" type="button" class="btn btn-link" data-toggle="collapse" :data-target="'#'+toggleId" ref="collapseButton"><div class="c-card__title"><slot name="header"></slot></div></button>
      <div v-else class="c-card__title"><slot name="header"></slot></div>
    </div>
    <div v-if="isCollapse" :id="toggleId" class="collapse">
      <div class="c-card__body">
        <slot name="body"></slot>
      </div>
    </div>
    <div v-else class="c-card__body">
      <slot name="body"></slot>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'card',
    props: {
      color: String,
      isCollapse: Boolean
    },
    data: function () {
      return {
        id: null,
        toggleId: null
      }
    },
    computed: {
      style_class: function () {
        return 'c-card c-card--' + (this.color || 'default');
      }
    },
    mounted: function () {
      var vm = this;//, comp = $(vm.$el);
      vm.id = 'card_container_' + vm._.uid;
      vm.toggleId = 'toggle_card_' + vm._.uid;

      //console.warn("Card container is Mounted", vm.id);
    }
  };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
    .c-card__header--collapse:hover {
        cursor: pointer;
    }
</style>
