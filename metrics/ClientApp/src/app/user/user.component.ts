import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import {State} from '@progress/kendo-data-query';
import {DataStateChangeEvent, GridDataResult} from '@progress/kendo-angular-grid';
import { UserService } from '../services/user.service';
import {VkUser} from "./VkResponse";

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class UserComponent implements OnInit {
  public data: GridDataResult;
  public users: VkUser[] = [];
  public userId: {FullName: '', UserId: ''};
  public search = '';
  public loading = false;
  public state: State = {
    skip: 0,
    take: 100
  };
  constructor(private userService: UserService) {}

  ngOnInit() {
    this.userService.getUsers().subscribe(data => this.users = data.data);
  }

  handleSearch() {
    this.loading = true;
    this.userService
      .getReposts(this.userId.UserId, this.state, this.search)
      .subscribe(data => {
        this.data = data;
        this.loading = false;
      });
  }

  onStateChange(state: DataStateChangeEvent) {
    this.state.skip = state.skip;
    this.state.take = state.take;
    this.handleSearch();
  }

  repost(owner_id: number, id: number, element: HTMLElement) {
    this.userService.repost(owner_id, id);
    element.classList.remove('k-i-fav-outline');
    element.classList.add('text-danger');
    element.classList.add('k-i-fav');
  }
}
