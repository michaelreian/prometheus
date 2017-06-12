import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PicaroonService } from './picaroon.service';
import * as _ from "lodash";

@Component({
  selector: 'app-picaroon-search-bar',
  templateUrl: './picaroon-search-bar.component.html',
  styleUrls: ['./picaroon-search-bar.component.css']
})
export class PicaroonSearchBarComponent implements OnInit {

  @Output() ready: EventEmitter<any> = new EventEmitter(true);

  sub: any;

  categories: any[];
  subcategories: any[];

  keywords: string;
  page: number;
  direction: string;
  orderBy: string;

  selectedCategory: string;
  selectedSubcategory: string;

  constructor(private service: PicaroonService, private router: Router, private route: ActivatedRoute) { }

  async ngOnInit() {
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

    this.sub = this.route
      .queryParams
      .subscribe(async params => {
        this.keywords = params['k'];
        this.selectedCategory = params['c'] || 0;
        this.selectedSubcategory = params['s'] || 0;
        this.page = +params['p'] || 0;
        this.direction = params['d'] || "Ascending";
        this.orderBy = params['o'] || "Uploaded";

        this.updateSubcategories(this.selectedCategory, this.selectedSubcategory);

        this.ready.emit(this);
      });
  }

  ngOnDestroy() {
    if(this.sub != null) {
      this.sub.unsubscribe();
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
    this.router.navigate(['/picaroon/search'], {
      queryParams: {
        k: this.keywords,
        c: this.selectedCategory,
        s: this.selectedSubcategory,
        p: this.page,
        d: this.direction,
        o: this.orderBy
      }
    });
  }
}
