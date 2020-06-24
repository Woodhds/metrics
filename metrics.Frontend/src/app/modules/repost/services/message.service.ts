import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {Message} from '../models/Message';
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
    return this.httpClient.get<DataSourceResponse<Message>>(`${environment.apiUrl}/message/${page}/${pageSize}`);
  }

  save(data: Message): Observable<any> {
    return this.httpClient.post(`${environment.apiUrl}/message`, data);
  }

  delete(id: number) {
    return this.httpClient.delete(`${environment.apiUrl}/message/${id}`)
  }

  setType(id: number, ownerId: number, categoryId: number) {
    return this.httpClient.post(`${environment.apiUrl}/message/type`, {
      MessageId: id,
      OwnerId: ownerId,
      MessageCategory: categoryId
    })
  }
}
