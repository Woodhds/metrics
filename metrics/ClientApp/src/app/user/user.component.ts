import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import {State} from '@progress/kendo-data-query';
import {DataStateChangeEvent, GridDataResult} from '@progress/kendo-angular-grid';
import { UserService } from '../services/user.service';
import {VkMessage, VkRepostModel, VkUser} from "./VkResponse";
import {NotificationService} from "@progress/kendo-angular-notification";

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
  public timeout: number = 15;
  public loading = false;
  public selectedKeys: any[] = [];
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

  onStateChange(state: DataStateChangeEvent) {
    this.state.skip = state.skip;
    this.state.take = state.take;
    this.handleSearch();
  }

  repost(repost: VkRepostModel[], element: HTMLElement) {
    this.loading = true;
    this.userService.repost(repost).subscribe(z => {
      if (z) {
        this.notificationService.show(
          {content: 'Выполнено успешно',
            position: { vertical: 'bottom', horizontal: 'right' },
            type: { style: 'success', icon: true }
          });
        element.classList.remove('k-i-fav-outline');
        element.classList.add('text-danger');
        element.classList.add('k-i-fav');
        this.loading = true;
      }
    });
  }
}
