import { Injectable } from '@angular/core';
import axios from 'axios';
const UrlAssembler = require('safe-url-assembler');

@Injectable()
export class PicaroonService {

  initialized: boolean = false;

  baseUrl: string = "http://localhost:5000/api";
  api = UrlAssembler(this.baseUrl).segment("/v1/picaroon");
  proxyUrl: string;

  constructor() { }

  public async initialize() {
    if(!this.initialized) {
      console.log('Initializing PicaroonService...');
      var resource = this.api.segment("/proxy");
      var response = await axios.get(resource.toString());
      this.proxyUrl = response.data;
      this.initialized = true;
    }
  }

  public async getCategories() {
    var resource = this.api.segment("/categories").query("proxyUrl", this.proxyUrl);
    var response = await axios.get(resource);
    return response.data;
  }

  public async search(keywords: string, categoryID: string, page: number, direction: string, orderBy: string) {
    var resource = this.api.segment("/search")
    .query("keywords", keywords)
    .query("categoryID", categoryID)
    .query("proxyUrl", this.proxyUrl)
    .query("page", page)
    .query("direction", direction)
    .query("orderBy", orderBy);

    var response = await axios.get(resource);
    return response.data;
  }

  public async getDetail(torrentID: string) {
    var resource = this.api.segment("/detail")
    .query("proxyUrl", this.proxyUrl)
    .query("torrentID", torrentID);

    var response = await axios.get(resource);
    return response.data;
  }
}
