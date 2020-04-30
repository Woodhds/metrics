import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {Message} from '../models/message';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../../../environments/environment';
import {DataSourceResponse} from '../models/DataSourceResponse';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  constructor(private httpClient: HttpClient) {
  }

  getTypes(page: number, pageSize: number): Observable<DataSourceResponse<Message>> {
    return this.httpClient.get<DataSourceResponse<Message>>(`${environment.apiUrl}/message/types/${page}/${pageSize}`);
  }
}
