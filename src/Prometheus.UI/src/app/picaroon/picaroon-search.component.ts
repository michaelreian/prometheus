import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { PicaroonService } from './picaroon.service';
import * as _ from "lodash";
const uuid = require('uuid/v4');

@Component({
  selector: 'app-picaroon-search',
  templateUrl: './picaroon-search.component.html',
  styleUrls: ['./picaroon-search.component.css']
})
export class PicaroonSearchComponent implements OnInit {

  searchResults = null;
  loading: boolean = true;
  error: any = null;


  orderedBy: string;
  sortedBy: string;

  subscription: any;
  parameters: any;

  constructor(private service: PicaroonService, private router: Router, private route: ActivatedRoute) { }

  async ngOnInit() {
    await this.service.initialize();

    this.subscription = this.route
      .queryParams
      .subscribe(async params => {
        this.parameters = params;
      });
  }

  ngOnDestroy() {
    if(this.subscription != null) {
      this.subscription.unsubscribe();
    }

  }

  async onReady(e) {
    this.error = null;
    this.loading = true;

    this.sortedBy = e.sortedBy;
    this.orderedBy = e.orderedBy;

    try {
      var results = await this.service.search(e.keywords, e.selectedSubcategory, e.page, e.sortedBy, e.orderedBy);
      this.searchResults = results;
    } catch(exception) {
      this.searchResults = null;
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
    } finally {
      this.loading = false;
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

  orderBy(orderedBy: string) {
    var parameters = _.cloneDeep(this.parameters);
    _.merge(parameters, { o: orderedBy });
    this.router.navigate(['/picaroon/search'], {
      queryParams: parameters
    });
  }

  sortBy(sortedBy: string) {
    var parameters = _.cloneDeep(this.parameters);
    _.merge(parameters, { d: sortedBy });
    this.router.navigate(['/picaroon/search'], {
      queryParams: parameters
    });
  }

  refresh() {
    var parameters = _.cloneDeep(this.parameters);
    _.merge(parameters, { refresh: uuid() });
    this.router.navigate(['/picaroon/search'], {
      queryParams: parameters
    });
  }
}
