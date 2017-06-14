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
  loading: boolean = false;

  orderedBy: string;
  sortedBy: string;

  constructor(private service: PicaroonService, private router: Router) { }

  async ngOnInit() {
    await this.service.initialize();
  }

  async onReady(e) {
    this.sortedBy = e.sortedBy;
    this.orderedBy = e.orderedBy;

    var results = await this.service.search(e.keywords, e.selectedSubcategory, e.page, e.sortedBy, e.orderedBy);

    this.searchResults = results;
  }

  browse(categoryID: string, subcategoryID: string) {
    this.router.navigate(['/picaroon/browse'], {
      queryParams: {
        c: categoryID,
        s: subcategoryID
      }
    });
  }

  orderBy(orderedBy: string) {
    this.orderedBy = orderedBy;
  }

  sortBy(sortedBy: string) {
    this.sortedBy = sortedBy;
  }

}
