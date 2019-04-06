<template>
    
</template>

<script lang="ts">
const enum Status {
    pending,
    loading,
    loaded,
    failed
}


import Vue from 'vue'
import Component from 'vue-class-component';
import { Prop, Watch } from 'vue-property-decorator';

@Component
export default class Image extends Vue {
    @Prop({type: string, default: '', required: false}) src: string;
    status: Status;
    img: Image;


    @Watch('src')
    setSrc(value) {
        this.status = value ? Status.loading : Status.pending
    }

    updated() {
        
    }
}
</script>
