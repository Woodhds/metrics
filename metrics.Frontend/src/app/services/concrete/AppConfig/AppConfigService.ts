import {IAppConfigService} from '../../abstract/IAppConfigService';
import {Injectable} from '@angular/core';
import {AppConfig} from '../../../models/AppConfig';
import {BehaviorSubject, Observable} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AppConfigService extends IAppConfigService {
  private config: AppConfig;
  private readonly configSubject: BehaviorSubject<AppConfig>;

  constructor(private httpClient: HttpClient) {
    super();
    this.configSubject = new BehaviorSubject<AppConfig>(null);
  }

  getConfig() : Observable<AppConfig> {
    if (this.config) {
      return this.configSubject;
    }

    return this.httpClient.get<AppConfig>(`${environment.apiUrl}/app`).pipe(map(source => {
      this.config = source;
      this.configSubject.next(source);
      return source;
    }));
  }
}
