import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../../environments/environment";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class RepostService {

  constructor(private httpClient: HttpClient) {

  }

  public executeNextRepost(): Observable<any> {
    return this.httpClient.post(`${environment.apiUrl}/user/next`, {})
  }
}
