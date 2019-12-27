import {IAppConfigService} from "../../abstract/IAppConfigService";
import {Injectable} from "@angular/core";
import {AppConfig} from "../../../models/AppConfig";

@Injectable({
  providedIn: 'root'
})
export class AppConfigService extends IAppConfigService {
  config: AppConfig;

  getConfig(key) {

  }
}
