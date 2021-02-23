import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-vk-image',
  templateUrl: './vk-image.component.html',
  styleUrls: ['./vk-image.component.scss']
})
export class VkImageComponent implements OnInit {
  @Input() src: string[];
  currentImageSubject: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  currentImage = 0;
  @ViewChild('img') img: ElementRef;

  constructor() {}

  ngOnInit(): void {
    this.currentImageSubject.subscribe(f => {
      this.currentImage = f;
      this.assignSrc();
    });
  }

  get totalImage() {
    return this.src.length;
  }

  get imagePoints() {
    return Array.from(Array(this.totalImage).keys());
  }

  next() {
    if (this.currentImage < this.totalImage - 1) {
      this.currentImageSubject.next(++this.currentImage);
    }
  }

  prev() {
    if (this.currentImage > 0) {
      this.currentImageSubject.next(--this.currentImage);
    }
  }

  private assignSrc(): void {
    if (!this.img) {return;}
    const el = this.img.nativeElement as HTMLImageElement;

    if (!el) {return;}
    el.src = this.srcImg;
  }

  setCurrent(image: number) {
    this.currentImageSubject.next(image);
  }

  get srcImg(): string {
    return this.src.length > 0
      ? this.src[this.currentImage]
      : 'assets/images/nophoto.png';
  }
}
