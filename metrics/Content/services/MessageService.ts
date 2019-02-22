import {DataSourceResponse, VkMessage, VkRepostModel} from "../models/VkMessage";
import axios, {AxiosPromise} from 'axios';


export function searchMessages(search: string, userId: number, page: number, pageSize: number): AxiosPromise<DataSourceResponse<VkMessage>> {
  return axios.get<DataSourceResponse<VkMessage>>(`api/repost/user?search=${search}
                    &userId=${userId}
                    &page=${page}&pageSize=${pageSize}`);
}

export function getFromSite() : AxiosPromise<DataSourceResponse<VkMessage>> {
  return axios.get<DataSourceResponse<VkMessage>>(`api/repost/site`);
}

export function repost(model: VkRepostModel[], timeout: number = 0) : AxiosPromise {
  return axios.post(`/api/repost/repost?timeout=${timeout}`, model);
}

export function like(model: VkRepostModel): AxiosPromise {
  return axios.get(`/api/repost/like?owner_id=${model.Owner_Id}&id=${model.Id}`);
}