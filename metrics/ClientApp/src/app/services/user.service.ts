import { Injectable } from '@angular/core';
import { State, toDataSourceRequestString } from '@progress/kendo-data-query';
import {HttpClient, HttpParams} from '@angular/common/http';
import { Observable } from 'rxjs';
import {VkResponse, VkMessage, VkRepostModel, VkUser} from '../user/VkResponse';
import { map } from 'rxjs/operators/map';
import { GridDataResult } from '@progress/kendo-angular-grid';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(private httpClient: HttpClient) { }

  getReposts(userId: string, state: State, search: string = null): Observable<GridDataResult> {
    return this.httpClient.get<VkResponse<VkMessage[]>>(`/api/repost/user?userId=${userId}&search=${search}&${toDataSourceRequestString(state)}`)
      .pipe(map(value => <GridDataResult>{ data: value['Data'], total: value['Total'] }));
  }

  repost(model: VkRepostModel[], timeout: number): Observable<boolean> {
    return this.httpClient.post<boolean>(`api/repost/repost?timeout=${timeout}`, model);
  }

  getUsers() : Observable<GridDataResult> {
    const params = new HttpParams({ fromObject: { columns: [ 'FullName', 'UserId', 'Avatar' ]} });
    return this.httpClient.get<VkUser[]>(`/api/VkUser?pageSize=50&page=1`, { params: params })
      .pipe(map(data => (<GridDataResult> { data: data['Data'], total: data['Total'] })));
  }
}
