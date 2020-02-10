import {Component, ElementRef, Input, OnChanges, OnInit, SimpleChanges} from '@angular/core';

@Component({
  selector: 'app-vk-image',
  templateUrl: './vk-image.component.html',
  styleUrls: ['./vk-image.component.scss']
})
export class VkImageComponent implements OnInit {

  @Input() src: string[];

  observer: IntersectionObserver;
  isLoad: Boolean = true;
  currentImage: number = 0;

  constructor(private elementRef: ElementRef) {
  }

  ngOnInit(): void {
    this.observer = new IntersectionObserver(entries => {
      entries.forEach(entry => {
        const {isIntersecting} = entry;
        if (isIntersecting) {
          this.isLoad = false;
          this.observer.disconnect()
        }
      });

      this.observer.observe(this.elementRef.nativeElement)
    })
  }

  get totalImage() {
    return this.src.length;
  }

  get imagePoints() {
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
}
