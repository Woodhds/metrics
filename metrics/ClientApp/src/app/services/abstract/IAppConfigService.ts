import {AppConfig} from "../../models/AppConfig";

export abstract class IAppConfigService {
  abstract getConfig() : AppConfig;
}
