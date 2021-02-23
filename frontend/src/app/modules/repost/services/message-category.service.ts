import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Message } from '../models/Message';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { DataSourceResponse } from '../models/DataSourceResponse';

@Injectable({
  providedIn: 'root',
})
export class MessageCategoryService {
  private routePrefix = `${environment.apiUrl}/messagecategory`;

  constructor(private httpClient: HttpClient) {}

  getTypes(
    page: number,
    pageSize: number
  ): Observable<DataSourceResponse<Message>> {
    return this.httpClient.get<DataSourceResponse<Message>>(
      `${this.routePrefix}/${page}/${pageSize}`
    );
  }

  save(data: Message): Observable<any> {
    return this.httpClient.post(`${this.routePrefix}`, data);
  }

  saveList(data: Array<Message>): Observable<Array<Message>> {
    return this.httpClient.post<Array<Message>>(`${this.routePrefix}/list`, data);
  }

  delete(id: number) {
    return this.httpClient.delete(`${this.routePrefix}/${id}`);
  }
}
