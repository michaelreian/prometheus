import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PicaroonService } from './picaroon.service';

@Component({
  selector: 'app-picaroon-detail',
  templateUrl: './picaroon-detail.component.html',
  styleUrls: ['./picaroon-detail.component.css']
})
export class PicaroonDetailComponent implements OnInit {

  sub: any;

  torrentID: string;

  constructor(private service: PicaroonService, private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {
    this.sub = this.route
      .params
      .subscribe(params => {
        this.torrentID = params['id'];
      });
  }

  ngOnDestroy() {
    if(this.sub != null) {
      this.sub.unsubscribe();
    }
  }

}
