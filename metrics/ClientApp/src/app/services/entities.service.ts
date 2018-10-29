import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ViewConfig} from '../entities/view-config.model';
import { Observable } from 'rxjs';
import { State, toDataSourceRequestString } from '@progress/kendo-data-query';
import { GridDataResult } from '@progress/kendo-angular-grid';

@Injectable({
  providedIn: 'root'
})
export class EntitiesService {

  constructor(private httpClient: HttpClient) {
  }

  getConfig(config: string): Observable<ViewConfig> {
    return this.httpClient.get<ViewConfig>(`/api/${config}/config`);
  }

  getData(config: string, state: State): Observable<GridDataResult> {
    return this.httpClient.get<GridDataResult>(`/api/${config}?${toDataSourceRequestString(state)}`);
  }
}
