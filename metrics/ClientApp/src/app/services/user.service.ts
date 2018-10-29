import { Injectable } from '@angular/core';
import { State, toDataSourceRequestString } from '@progress/kendo-data-query';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { VkResponse, VkMessage, VkRepostModel } from '../user/VkResponse';
import { map } from 'rxjs/operators/map';
import { GridDataResult } from '@progress/kendo-angular-grid';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(private httpClient: HttpClient) { }

  getReposts(userId: string, state: State, search: string = null): Observable<GridDataResult> {
    return this.httpClient.get<VkResponse<VkMessage[]>>(`/api/repost/user?userId=${userId}&search=${search}&${toDataSourceRequestString(state)}`)
      .pipe(map(value => <GridDataResult>{ data: value['data'], total: value['total'] }));
  }

  repost(owner_id: number, id: number): void {
    this.httpClient.post(`api/repost/repost`, [ new VkRepostModel(owner_id, id) ]).subscribe();
  }
}
