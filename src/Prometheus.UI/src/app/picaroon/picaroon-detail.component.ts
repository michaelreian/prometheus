import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PicaroonService } from './picaroon.service';
import * as _ from "lodash";
const uuid = require('uuid/v4');

@Component({
  selector: 'app-picaroon-detail',
  templateUrl: './picaroon-detail.component.html',
  styleUrls: ['./picaroon-detail.component.css']
})
export class PicaroonDetailComponent implements OnInit {

  subscription: any;

  torrentID: string;
  detail: any;

  loading: boolean = true;
  error: any = null;

  parameters: any;

  constructor(private service: PicaroonService, private router: Router, private route: ActivatedRoute) { }

  async ngOnInit() {

    await this.service.initialize();

    this.subscription = this.route
      .params
      .subscribe(async params => {

        this.parameters = params;

        try {
          this.loading = true;

          this.torrentID = params['id'];

          this.detail = await this.service.getDetail(this.torrentID);

          console.log(this.detail);
        } catch(exception) {
          this.detail = null;
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

      });
  }

  ngOnDestroy() {
    if(this.subscription != null) {
      this.subscription.unsubscribe();
    }
  }

  refresh() {
    var parameters = _.cloneDeep(this.parameters);
    _.merge(parameters, { refresh: uuid() });
    console.log(parameters);
    this.router.navigate(['/picaroon/detail'], {
      queryParams: parameters
    });
  }
}
