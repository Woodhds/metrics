import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ViewConfig } from '../entities/view-config.model';

@Component({
  selector: 'app-addentity',
  templateUrl: './addentity.component.html',
  styleUrls: ['./addentity.component.scss']
})
export class AddentityComponent implements OnInit {
  @Output() save: EventEmitter<any> = new EventEmitter<any>();
  @Input() viewConfig: ViewConfig;
  constructor() { }

  ngOnInit() {
  }

  saveEntity() {
  }
}
