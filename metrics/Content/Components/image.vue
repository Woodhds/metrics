<template>
<VObserver @intersect="intersect">
  <div class="flex flex-col items-center post_image">
      <div class="flex items-center">
          <a @click="prev" v-if="totalImage > 1" class="border border-gray-300 hover:shadow rounded-full block cursor-pointer p-4 mr-6">
              <svg class="fill-current h-4 w-4" xmlns:xlink="http://www.w3.org/1999/xlink">
              <use xlink:href="/images/icons.svg#arrow-left"></use>
              </svg>
          </a>
          <div class="flex flex-col mb-2">
              <div v-if="!isLoad" class="lds-ripple"><div></div><div></div></div>
              <img v-else class="w-auto h-32 mb-2" :src="isLoad && src.length > 0 ? src[currentImage] : `images/nophoto.jpg`"/>
              <ul class="flex flex-wrap items-center justify-center">
              <li @click="setCurrent(image)" v-for="image of imagePoints" :key="image" class="rounded-full relative border border-gray-300 h-4 w-4 p-1 ml-2 cursor-pointer">
                  <span :style="{transform: 'translateX(' + (currentImage * 24) + 'px)' }" v-if="image === 0" class="bg-gray-500 top-0 image-point left-0 w-3 h-3 absolute rounded-full block"></span>
              </li>
              </ul>
          </div>
          <a @click="next" v-if="totalImage > 1" class="border border-gray-300 hover:shadow rounded-full block cursor-pointer p-4 ml-6">
              <svg class="fill-current h-4 w-4" xmlns:xlink="http://www.w3.org/1999/xlink">
              <use xlink:href="/images/icons.svg#arrow-right"></use>
              </svg>
          </a>
      </div>
      <div v-html="figcaption" class="text-sm leading-normal word-break max-h-screen overflow-y-auto">
      </div>
  </div>
</VObserver>
</template>

<script lang="ts">

import Vue from 'vue'
import Component from 'vue-class-component';
import { Prop, Watch } from 'vue-property-decorator';
import VObserver from './Observer.vue';

@Component({
  components: { VObserver }
})
export default class Image extends Vue {
    @Prop({type: Array, default: [], required: true}) src: string[];
    @Prop({type: String, default: '', required: false}) figcaption: string;
    currentImage: number = 0;
    observer: IntersectionObserver;
    isLoad: boolean = false;

    get totalImage(): number {
      return this.src.length;
    }

    get imagePoints(): number[] {
      return Array.from(Array(this.totalImage).keys());
    }

    next() {
      if (this.currentImage < this.totalImage - 1) {
        this.currentImage++;
      }
    }

    prev() {
      if (this.currentImage > 0) {
        this.currentImage--;
      }
    }
    
    setCurrent(image: number) {
      this.currentImage = image;
    }

    intersect(observer:IntersectionObserver) {
      this.isLoad = true;
      observer.disconnect();
    }
}
</script>
