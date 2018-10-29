import { Component, OnInit, Input, Output } from '@angular/core';
import { ViewConfig } from '../entities/view-config.model';
import { EventEmitter } from 'protractor';

@Component({
  selector: 'app-addentity',
  templateUrl: './addentity.component.html',
  styleUrls: ['./addentity.component.scss']
})
export class AddentityComponent implements OnInit {
  @Output() save: EventEmitter = new EventEmitter();
  @Input() viewConfig: ViewConfig;
  constructor() { }

  ngOnInit() {
  }

  saveEntity() {
  }
}
