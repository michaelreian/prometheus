import { Component, OnInit } from '@angular/core';
import { PicaroonService } from './picaroon.service';
import * as _ from "lodash";

@Component({
  selector: 'app-picaroon',
  templateUrl: './picaroon.component.html',
  styleUrls: ['./picaroon.component.css']
})
export class PicaroonComponent implements OnInit {

  categories;
  subcategories;

  keywords: string = null;
  page: number = 0;
  direction: string = "Ascending";
  orderBy: string = "Uploaded";

  selectedCategory;
  selectedSubcategory;

  constructor(public service: PicaroonService) { }

  onCategoryChange(e) {
    var categoryID = e.target.value;
    this.updateSubcategories(categoryID);
  }

  updateSubcategories(categoryID: string) {
    var category = _.find(this.categories, (c:any) => c.id == categoryID);

    this.subcategories = category.subcategories;
    this.selectedSubcategory = categoryID;
  }

  async ngOnInit() {
    await this.service.initialize();
    this.categories = await this.service.getCategories();
    this.selectedCategory = "0";
    this.updateSubcategories(this.selectedCategory);
  }

  async search() {
    var results = await this.service.search(this.keywords, this.selectedSubcategory, this.page, this.direction, this.orderBy);

    console.log(results);
  }
}
