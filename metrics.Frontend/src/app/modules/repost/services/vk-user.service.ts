import {Injectable, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import VkUserModel from "../models/VkUserModel";
import { environment } from '../../../../environments/environment'
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class VkUserService implements OnInit{
  constructor(private httpClient: HttpClient) { }

  ngOnInit(): void {
  }

  getUsers(search: string = '') : Observable<VkUserModel[]> {
    return this.httpClient.get<VkUserModel[]>(`${environment.apiUrl}/user`, { params: {searchStr: search}});
  }

  createUser(id: string) {
    this.httpClient.post(`${environment.apiUrl}/user`, null, {
      params: {
        userId: id
      }
    })
      .subscribe(() => {
        this.getUsers()
      })
  }
}
