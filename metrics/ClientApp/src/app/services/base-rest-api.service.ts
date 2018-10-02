import { Injectable } from '@angular/core';
import {BehaviorSubject, Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {GridDataResult} from "@progress/kendo-angular-grid";
import { toDataSourceRequestString } from '@progress/kendo-data-query'

@Injectable({
  providedIn: 'root'
})
export class BaseRestApiService extends BehaviorSubject<GridDataResult> {

  Base_url: string = 'https://localhost:5001/';
  constructor(private httpClient: HttpClient, protected entity: string) {
    super(null);
  }

  public query(state: any): void {
    this.fetch(this.entity, state)
      .subscribe(x => super.next(x));
  }

  protected fetch(entity: string, state: any): Observable<GridDataResult> {
    const queryStr = `${toDataSourceRequestString(state)}`;
    return this.httpClient.get<GridDataResult>(`${this.Base_url}${entity}?${queryStr}`);
  }
}
