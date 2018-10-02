import { Injectable } from '@angular/core';
import {BaseRestApiService} from "../services/base-rest-api.service";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class ProductService extends BaseRestApiService {
  constructor(http: HttpClient) {
    super(http, 'product')
  }
}
