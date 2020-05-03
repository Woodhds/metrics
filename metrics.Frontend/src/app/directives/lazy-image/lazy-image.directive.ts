import { Directive, ElementRef, Input } from '@angular/core';

@Directive({
  selector: '[appLazyImage]'
})
export class LazyImageDirective {
  constructor(elementRef: ElementRef) {
    const observer = new IntersectionObserver((entries, imgObserver) => {
      entries.forEach(x => {
        if (x.isIntersecting) {
          const el = elementRef.nativeElement as HTMLImageElement;
          el.src = this.appLazyImage;
          imgObserver.unobserve(x.target);
        }
      });
    });

    observer.observe(elementRef.nativeElement);
  }

  @Input('appLazyImage') appLazyImage: string;
}
