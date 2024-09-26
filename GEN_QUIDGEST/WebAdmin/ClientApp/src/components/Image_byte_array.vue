<template>
    <img :src="getSrc" />
</template>

<script>
  export default {
    name: 'image-byte-array',
    props: {
      modelValue: String
    },
    data: function () {
      return {
        id: null
      }
    },
    computed: {
        getSrc: function () {
            if ($.isEmptyObject(this.modelValue)) {
                return '';
            }
            return 'data:image/jpeg;base64,' + btoa(
                new Uint8Array(Array.from(this.modelValue))
                    .reduce((data, byte) => data + String.fromCharCode(byte), '')
            );
        }
    },
    mounted: function () {
      var vm = this;//, comp = $(vm.$el);
      vm.id = "img_n_" + vm._.uid;

      //console.warn("Image Mounted");
    },
  };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
