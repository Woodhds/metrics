import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import {State} from '@progress/kendo-data-query';
import {DataStateChangeEvent, GridDataResult, SelectionEvent} from '@progress/kendo-angular-grid';
import { UserService } from '../services/user.service';
import {VkMessage, VkUser} from "./VkResponse";
import {NotificationService} from "@progress/kendo-angular-notification";
import {NotificationSettings} from "@progress/kendo-angular-notification/dist/es2015/models/notification-settings";

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class UserComponent implements OnInit {
  public data: GridDataResult;
  public users: VkUser[] = [];
  public selected: VkMessage[] = [];
  public userId: {FullName: '', UserId: ''};
  public search = '';
  public loading = false;
  public state: State = {
    skip: 0,
    take: 100
  };
  constructor(private userService: UserService, private notificationService: NotificationService) {}

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

  onSelectionChange(event: SelectionEvent) {
    console.log(event);
  }

  onStateChange(state: DataStateChangeEvent) {
    this.state.skip = state.skip;
    this.state.take = state.take;
    this.handleSearch();
  }

  repost(owner_id: number, id: number, element: HTMLElement) {
    this.userService.repost(owner_id, id).subscribe(z => {
      if (z) {
        this.notificationService.show(
          {content: 'Выполнено успешно',
            position: { vertical: 'bottom', horizontal: 'right' },
            type: { style: 'success', icon: true }
          });
        element.classList.remove('k-i-fav-outline');
        element.classList.add('text-danger');
        element.classList.add('k-i-fav');

      }
    });

  }
}
