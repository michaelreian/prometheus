import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PicaroonService } from './picaroon.service';

@Component({
  selector: 'app-picaroon-browse',
  templateUrl: './picaroon-browse.component.html',
  styleUrls: ['./picaroon-browse.component.css']
})
export class PicaroonBrowseComponent implements OnInit {

  categories: any[];

  constructor(private service: PicaroonService, private router: Router) { }

  async ngOnInit() {
    await this.service.initialize();
    this.categories = await this.service.getCategories();
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
