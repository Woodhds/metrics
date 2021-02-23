import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  get backToTopVisible(): boolean {
    return window.scrollY > 250;
  }

  backToTop() {
    window.scroll({
      top: 0,
      behavior: 'smooth'
    });
  }
}
