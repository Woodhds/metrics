import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {DataSourceResponse, VkMessage} from "../models/VkMessageModel";
import {environment} from "../../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class VkMessageService {

  constructor(private httpClient: HttpClient) {
  }

  public getFromUser(userId: string, page: number = 1, pageSize: number = 50, search: string = '') : Observable<DataSourceResponse<VkMessage>> {
    return this.httpClient.get<DataSourceResponse<VkMessage>>(`${environment.apiUrl}/repost/user`,
      {
        params: new HttpParams()
          .set('userId', userId)
          .set('page', page.toString())
          .set('pageSize', pageSize.toString())
          .set('searchStr', search)
      }
    )
  }
}
