import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DataSourceResponse } from '../../repost/models/DataSourceResponse';
import { UserMessage } from '../models/UserMessage';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UserMessageService {
  private routePrefix = `${environment.apiUrl}/usermessage`;

  constructor(private httpClient: HttpClient) {}

  get(
    page: number,
    pageSize: number
  ): Observable<DataSourceResponse<UserMessage>> {
    return this.httpClient.get<DataSourceResponse<UserMessage>>(
      `${this.routePrefix}`,
      {
        params: new HttpParams({
          fromObject: {
            page: page.toString(),
            pageSize: pageSize.toString(),
          },
        }),
      }
    );
  }
}
