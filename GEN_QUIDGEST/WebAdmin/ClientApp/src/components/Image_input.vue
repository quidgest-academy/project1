<template>
    <div class="i-image">
        <div class="d-flex" v-if="label"><label class="i-text__label" :for="id">{{ label }}</label></div>
        <img :src="_src" />
        <template v-if="!isReadOnly && !isEmptyObject(setUrl)">
            <div class="row">
                <div class="col-12">
                    <input type="file" @change="onFileChange">
                </div>
            </div>
        </template>
    </div>
</template>

<script>
    import { reusableMixin } from '@/mixins/mainMixin';
    import bootbox from 'bootbox';
    export default {
        name: 'image-input',
        emits: ['updateSrc'],
        props: {
            getUrl: String,
            setUrl: String,
            label: String,
            isReadOnly: Boolean
        },
        data: function () {
            return {
                id: null,
                v: Date.now()
            }
        },
        computed: {
            _src: function () {
                let tSrc = typeof this.getUrl === "function" ? this.getUrl() : this.getUrl;
				return tSrc + (tSrc.indexOf('?') === -1 ? '?' : '&') + "v=" + this.v;
            }
        },
        mixins: [reusableMixin],
        methods: {
            onFileChange(e) {
                let vm = this;

                let files = e.target.files || e.dataTransfer.files;
                if (!files.length)
                    return;

                let formData = new FormData();
                formData.append('image', files[0]);

                jQuery.ajax({
                    url: vm.setUrl,
                    type: "POST",
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (result) {
                        if (result.Success) { vm.updateSrc(); }
                        else { bootbox.alert(result.Message); }
                    }
                });

                this.$emit('file-change', );
            },
            updateSrc: function () {
                this.v = Date.now();
                
                this.$emit('updateSrc');
            }
        },
        mounted: function () {
            var vm = this;
            vm.id = 'input_img_' + vm._.uid;
        },
    };
</script>