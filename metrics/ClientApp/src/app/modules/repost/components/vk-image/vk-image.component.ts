import { Component, ElementRef, Input, OnInit, ViewChild } from "@angular/core";

@Component({
  selector: "app-vk-image",
  templateUrl: "./vk-image.component.html",
  styleUrls: ["./vk-image.component.scss"]
})
export class VkImageComponent implements OnInit {
  @Input() src: string[];
  currentImage: number = 0;
  @ViewChild("img") img: ElementRef;

  constructor() {}

  ngOnInit(): void {}

  get totalImage() {
    return this.src.length;
  }

  get imagePoints() {
    return Array.from(Array(this.totalImage).keys());
  }

  next() {
    if (this.currentImage < this.totalImage - 1) {
      this.currentImage++;
      this.assignSrc();
    }
  }

  prev() {
    if (this.currentImage > 0) {
      this.currentImage--;
      this.assignSrc();
    }
  }

  private assignSrc() {
    let el = this.img.nativeElement as HTMLImageElement;
    el.src = this.srcImg;
  }

  setCurrent(image: number) {
    this.currentImage = image;
  }

  get srcImg(): string {
    return this.src.length > 0
      ? this.src[this.currentImage]
      : "assets/images/nophoto.png";
  }
}
