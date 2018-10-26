import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import {State} from '@progress/kendo-data-query';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss'],
  encapsulation: ViewEncapsulation.Emulated
})
export class UserComponent implements OnInit {
  public data: GridDataResult;
  public userId  = '';
  public search = '';
  public state: State = {
    skip: 0,
    take: 100
  };
  constructor(private userService: UserService) {}

  ngOnInit() {}

  handleSearch() {
    this.userService
      .getReposts(this.userId, this.state, this.search)
      .subscribe(data => {
        this.data = data;
      });
  }
}
