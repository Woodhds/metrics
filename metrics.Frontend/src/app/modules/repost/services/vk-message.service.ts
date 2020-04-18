import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {DataSourceResponse, VkMessage, VkRepostModel} from "../models/VkMessageModel";
import {environment} from "../../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class VkMessageService {

  private routePrefix: string = `${environment.apiUrl}/repost`;

  constructor(private httpClient: HttpClient) {
  }

  public get(page: number = 1, pageSize: number = 50, search: string = '', user: string = ''): Observable<DataSourceResponse<VkMessage>> {
    return this.httpClient.get<DataSourceResponse<VkMessage>>(`${this.routePrefix}/user`,
      {
        params: new HttpParams()
          .set('page', page.toString())
          .set('pageSize', pageSize.toString())
          .set('search', search)
          .set('user', user)
      }
    )
  }

  public repost(model: VkRepostModel[], timeout: number = 0) {
    return this.httpClient.post(`${this.routePrefix}/repost`, model, {params: new HttpParams().set('timeout', String(timeout))});
  }

  public like(owner_id: number, id: number) {
    return this.httpClient.get(`${this.routePrefix}/like`, {
      params: new HttpParams({})
        .set('owner_id', String(owner_id))
        .set('id', String(id))
    })
  }
}
