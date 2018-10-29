import { Component, OnInit } from '@angular/core';
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
  styleUrls: ['./entities.component.scss']
})
export class EntitiesComponent implements OnInit {
  public viewConfig: Observable<ViewConfig>;
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
    this.viewConfig = this.route.paramMap.pipe(
      switchMap((params: ParamMap) =>
        this.entitiesService
          .getConfig(params.get('entity'))
      )
    );
  }

  ngOnInit() {
    this.fetchData();
  }

  fetchData() {
    this.viewConfig.subscribe(z => {
      this.data = this.entitiesService.getData(z.name, this.state);
    });
  }

  addItem() {
    this.viewConfig.subscribe(z => {
      this.dialogService.open({
        content: AddentityComponent
      });
    });
  }
}
