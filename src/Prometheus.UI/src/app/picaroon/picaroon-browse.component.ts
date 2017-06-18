import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PicaroonService } from './picaroon.service';
const uuid = require('uuid/v4');

@Component({
  selector: 'app-picaroon-browse',
  templateUrl: './picaroon-browse.component.html',
  styleUrls: ['./picaroon-browse.component.css']
})
export class PicaroonBrowseComponent implements OnInit {

  error: any;

  categories: any[];

  constructor(private service: PicaroonService, private router: Router) { }

  async ngOnInit() {
    try {
      await this.service.initialize();
      this.categories = await this.service.getCategories();
    } catch(exception) {
      if(exception.response != null && exception.response.data != null) {
        this.error = exception.response.data;
      }
      else {
        this.error = {
          message: 'An unexpected error has occurred.',
          correlationID: uuid()
        };
      }
      console.log(this.error);
    }
  }

  browse(categoryID: string, subcategoryID: string) {
    this.router.navigate(['/picaroon/browse'], {
      queryParams: {
        c: categoryID,
        s: subcategoryID
      }
    });
  }

  refresh() {
    this.router.navigate(['/picaroon'], {
      queryParams: { refresh: uuid() }
    });
  }

}
