import { Component, OnInit } from '@angular/core';
import { PicaroonService } from './picaroon/picaroon.service';
declare var jQuery: any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  host: {
  '(window:resize)': 'onResize()'
}
})
export class AppComponent implements OnInit {
  public ngOnInit(): any {
    this.detectBody();
  }

  public onResize() {
    this.detectBody();
  }

  detectBody(): void {
    if (jQuery(document).width() < 769) {
      jQuery('body').addClass('body-small')
    } else {
      jQuery('body').removeClass('body-small')
    }
  }
}
