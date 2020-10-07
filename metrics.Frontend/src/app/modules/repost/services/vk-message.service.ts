import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { VkMessage, VkRepostModel } from "../models/VkMessageModel";
import { environment } from "../../../../environments/environment";
import { DataSourceResponse } from "../models/DataSourceResponse";

@Injectable({
  providedIn: "root",
})
export class VkMessageService {
  private routePrefix = `${environment.apiUrl}/message`;

  constructor(private httpClient: HttpClient) {}

  public get(
    page: number = 1,
    pageSize: number = 50,
    search: string = ""
  ): Observable<DataSourceResponse<VkMessage>> {
    return this.httpClient.get<DataSourceResponse<VkMessage>>(
      `${this.routePrefix}/user`,
      {
        params: new HttpParams()
          .set("page", page.toString())
          .set("pageSize", pageSize.toString())
          .set("search", search),
      }
    );
  }

  public repost(model: VkRepostModel[]) {
    return this.httpClient.post(`${this.routePrefix}/repost`, model);
  }

  public like(owner_id: number, id: number) {
    return this.httpClient.get(`${this.routePrefix}/like`, {
      params: new HttpParams({})
        .set("owner_id", String(owner_id))
        .set("id", String(id)),
    });
  }

  setType(id: number, ownerId: number, categoryId: number) {
    return this.httpClient.post(`${this.routePrefix}/type`, {
      MessageId: id,
      OwnerId: ownerId,
      MessageCategory: categoryId,
    });
  }
}
