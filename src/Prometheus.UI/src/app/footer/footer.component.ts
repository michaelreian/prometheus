import { Component, OnInit } from '@angular/core';

declare var APP_VERSION: string;

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {

  applicationVersion: string = APP_VERSION;

  constructor() { }

  ngOnInit() {
  }

}
