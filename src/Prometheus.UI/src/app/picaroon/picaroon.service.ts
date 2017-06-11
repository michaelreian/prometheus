import { Injectable } from '@angular/core';
import axios from 'axios';
const UrlAssembler = require('safe-url-assembler');

@Injectable()
export class PicaroonService {

  baseUrl: string = "http://localhost:5000/api";
  api = UrlAssembler(this.baseUrl).segment("/v1/picaroon");
  proxyUrl: string;

  constructor() { }

  public async initialize() {
    var resource = this.api.segment("/proxy");
    var response = await axios.get(resource.toString());
    this.proxyUrl = response.data;
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
}
