import {AppConfig} from '../../models/AppConfig';
import {Observable} from 'rxjs';

export abstract class IAppConfigService {
  abstract getConfig() : Observable<AppConfig>;
}
