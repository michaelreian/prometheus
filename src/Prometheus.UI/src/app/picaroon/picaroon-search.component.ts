import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PicaroonService } from './picaroon.service';

@Component({
  selector: 'app-picaroon-search',
  templateUrl: './picaroon-search.component.html',
  styleUrls: ['./picaroon-search.component.css']
})
export class PicaroonSearchComponent implements OnInit {

  searchResults = null;

  constructor(private service: PicaroonService, private router: Router) { }

  async ngOnInit() {
    await this.service.initialize();
  }

  async onReady(e) {
    this.searchResults = null;

    var results = await this.service.search(e.keywords, e.selectedSubcategory, e.page, e.direction, e.orderBy);

    this.searchResults = results;

    console.log(this.searchResults);
  }

  browse(categoryID: string, subcategoryID: string) {
    this.router.navigate(['/picaroon/browse'], {
      queryParams: {
        c: categoryID,
        s: subcategoryID
      }
    });
  }

}
