import {Component, HostListener, OnInit} from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { ViewConfig } from './view-config.model';
import { EntitiesService } from '../services/entities.service';
import { State } from '@progress/kendo-data-query';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Observable } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { AddentityComponent } from '../addentity/addentity.component';
import { DialogService } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'app-entities',
  templateUrl: './entities.component.html',
  styleUrls: ['./entities.component.scss'],
  entryComponents: [AddentityComponent]
})
export class EntitiesComponent implements OnInit {
  public viewConfig: ViewConfig;
  public data: Observable<GridDataResult>;
  public state: State = {
    skip: 0,
    take: 50
  };
  constructor(
    private route: ActivatedRoute,
    private entitiesService: EntitiesService,
    private dialogService: DialogService
  ) {
     this.route.paramMap.subscribe(
       (params: ParamMap) =>
        this.entitiesService
          .getConfig(params.get('entity')).subscribe((config: ViewConfig) => {
          this.viewConfig = config;
          this.fetchData();
        })
      );
  }

  ngOnInit() {

 }

  fetchData() {
    this.data = this.entitiesService.getData(this.viewConfig.Name, this.state, this.viewConfig.Columns);
  }

  addItem() {
      const dialog = this.dialogService.open({
        content: AddentityComponent,
        title: 'Добавить',
        height: 600,
        width: 800
      });

      const config = dialog.content.instance;
      config.viewConfig = this.viewConfig;
      config.onClose = this.fetchData();
  }
}
