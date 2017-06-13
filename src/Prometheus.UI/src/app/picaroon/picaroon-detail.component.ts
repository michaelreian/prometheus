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
  detail: any;

  constructor(private service: PicaroonService, private router: Router, private route: ActivatedRoute) { }

  async ngOnInit() {
    await this.service.initialize();

    this.sub = this.route
      .params
      .subscribe(async params => {
        this.torrentID = params['id'];

        this.detail = await this.service.getDetail(this.torrentID);

        console.log(this.detail);
      });
  }

  ngOnDestroy() {
    if(this.sub != null) {
      this.sub.unsubscribe();
    }
  }

}
