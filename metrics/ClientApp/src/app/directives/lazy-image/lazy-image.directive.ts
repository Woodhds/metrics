import { Directive, ElementRef, Input } from "@angular/core";

@Directive({
  selector: "[appLazyImage]"
})
export class LazyImageDirective {
  constructor(elementRef: ElementRef) {
    const observer = new IntersectionObserver((entries, imgObserver) => {
      entries.forEach(x => {
        if (x.isIntersecting) {
          let el = elementRef.nativeElement as HTMLImageElement;
          el.src = this.src;
          imgObserver.unobserve(elementRef.nativeElement);
        }
      });
    });

    observer.observe(elementRef.nativeElement);
  }

  @Input("appLazyImage") src: string;
}
