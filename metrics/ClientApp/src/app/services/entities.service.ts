import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {ColumnView, ViewConfig} from '../entities/view-config.model';
import {Observable} from 'rxjs';
import {State, toDataSourceRequestString} from '@progress/kendo-data-query';
import {GridDataResult} from '@progress/kendo-angular-grid';
import {map} from "rxjs/operators/map";

@Injectable({
  providedIn: 'root'
})
export class EntitiesService {

  constructor(private httpClient: HttpClient) {
  }

  getConfig(config: string): Observable<ViewConfig> {
    return this.httpClient.get<ViewConfig>(`/api/${config}/config`);
  }

  getData(config: string, state: State, columns: ColumnView[]): Observable<GridDataResult> {
    const columnsNames = columns.map(e => e.Name);
    let params = new HttpParams({
      fromObject: {
        columns: columnsNames
      }
    });

    return this.httpClient.get(`/api/${config}?${toDataSourceRequestString(state)}`, {params: params})
      .pipe(map(response => (<GridDataResult> { data: response['Data'], total: parseInt(response['Total']) })));
  }

  save(config: string, data: any): Observable<boolean> {
    return this.httpClient.post<boolean>(`/api/${config}`, data);
  }

  delete(config: string, id: number) : Observable<boolean> {
    return this.httpClient.delete<boolean>(`/api/${config}/${id}`);
  }
}
