import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import VkUserModel from '../models/VkUserModel';
import { environment } from '../../../../environments/environment';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class VkUserService {
  constructor(private httpClient: HttpClient) { }

  getUsers(search: string = ''): Observable<VkUserModel[]> {
    return this.httpClient.get<VkUserModel[]>(`${environment.apiUrl}/user`, { params: {searchStr: search}});
  }

  createUser(id: number) {
    this.httpClient.post(`${environment.apiUrl}/user`, null, {
      params: {
        userId: id.toString()
      }
    })
      .subscribe(() => {
        this.getUsers();
      });
  }

  search(str: string): Observable<VkUserModel[]> {
    return this.httpClient.get<VkUserModel[]>(`${environment.apiUrl}/user/search`, {
      params: new HttpParams({
        fromObject: {
          search: str
        }
      })
    });
  }
}
