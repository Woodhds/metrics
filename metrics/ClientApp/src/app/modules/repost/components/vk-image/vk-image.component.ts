import { Component, Input, OnInit } from "@angular/core";

@Component({
  selector: "app-vk-image",
  templateUrl: "./vk-image.component.html",
  styleUrls: ["./vk-image.component.scss"]
})
export class VkImageComponent implements OnInit {
  @Input() src: string[];
  isLoad: Boolean = true;
  currentImage: number = 0;

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

  get srcImg(): string {
    return this.isLoad && this.src.length > 0
      ? this.src[this.currentImage]
      : "assets/images/nophoto.png";
  }
}
