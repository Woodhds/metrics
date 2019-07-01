<template>
    <div>
        <slot></slot>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import { Emit } from 'vue-property-decorator';

@Component
export default class VObserver extends Vue{
    observer: IntersectionObserver;

    mounted() {
        var self = this;
        this.observer = new IntersectionObserver(entries => {
            entries.forEach(entry => {
                const { isIntersecting } = entry;
                if(isIntersecting) {
                    this.$emit('intersect', self.observer)
                }
            })
            
        })
        this.observer.observe(this.$el)
    }
    
}
</script>

