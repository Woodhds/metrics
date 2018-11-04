import {Component, OnInit, Input, EventEmitter, Output} from '@angular/core';
import {PropertyDataType, ViewConfig} from '../entities/view-config.model';
import {NgForm} from "@angular/forms";
import {EntitiesService} from "../services/entities.service";
import {DialogRef} from "@progress/kendo-angular-dialog";

@Component({
  selector: 'app-addentity',
  templateUrl: './addentity.component.html',
  styleUrls: ['./addentity.component.scss']
})
export class AddentityComponent implements OnInit {
  @Input() viewConfig: ViewConfig;
  public dataType = PropertyDataType;
  constructor(private entitiesService: EntitiesService, private dialog: DialogRef) { }

  ngOnInit() {
  }

  saveEntity(form:NgForm) {
    this.entitiesService.save(this.viewConfig.Name, form.value);
    this.dialog.close();
  }
}
