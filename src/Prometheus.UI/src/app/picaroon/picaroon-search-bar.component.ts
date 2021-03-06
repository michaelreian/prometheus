import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PicaroonService } from './picaroon.service';
import * as _ from "lodash";
const uuid = require('uuid/v4');

@Component({
  selector: 'app-picaroon-search-bar',
  templateUrl: './picaroon-search-bar.component.html',
  styleUrls: ['./picaroon-search-bar.component.css']
})
export class PicaroonSearchBarComponent implements OnInit {

  @Output() ready: EventEmitter<any> = new EventEmitter(true);

  error: any;

  subscription: any;

  categories: any[];
  subcategories: any[];

  keywords: string;
  page: number;
  @Input() sortedBy: string;
  @Input() orderedBy: string;

  selectedCategory: string;
  selectedSubcategory: string;

  constructor(private service: PicaroonService, private router: Router, private route: ActivatedRoute) { }

  async ngOnInit() {
    try {
      await this.service.initialize();

      this.categories = await this.service.getCategories();

      for (let category of this.categories) {
        if(category.subcategories) {
          category.subcategories.unshift({
            id: category.id,
            name: `Any ${category.name}`
          });
        }
      }

      this.categories.unshift({
        id: '0',
        name: 'Any',
        subcategories: [{
          id: '0',
          name: 'Any'
        }]
      });

      this.subscription = this.route
        .queryParams
        .subscribe(async params => {
          this.keywords = params['k'];
          this.selectedCategory = params['c'] || 0;
          this.selectedSubcategory = params['s'] || 0;
          this.page = +params['p'] || 0;
          this.sortedBy = params['d'] || "Descending";
          this.orderedBy = params['o'] || "Uploaded";

          this.updateSubcategories(this.selectedCategory, this.selectedSubcategory);

          this.ready.emit(this);
        });
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

  ngOnDestroy() {
    if(this.subscription != null) {
      this.subscription.unsubscribe();
    }

  }


  onCategoryChange(e) {
    var categoryID = e.target.value;
    this.updateSubcategories(categoryID, categoryID);
  }

  updateSubcategories(categoryID: string, subcategoryID: string) {
    var category = _.find(this.categories, (c: any) => c.id == categoryID);

    this.subcategories = category.subcategories;
    this.selectedSubcategory = subcategoryID || categoryID;
  }

  search() {
    var isSortable = !(this.selectedSubcategory === '0' && (this.keywords == null || this.keywords === ''));

    this.router.navigate(['/picaroon/search'], {
      queryParams: {
        k: this.keywords,
        c: this.selectedCategory,
        s: this.selectedSubcategory,
        p: isSortable ? this.page : null,
        d: isSortable ? this.sortedBy : null,
        o: isSortable ? this.orderedBy : null
      }
    });
  }
}
