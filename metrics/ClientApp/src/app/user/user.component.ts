import {Component, OnInit, ViewEncapsulation} from '@angular/core';
import {State} from '@progress/kendo-data-query';
import {DataStateChangeEvent, GridDataResult, SelectionEvent} from '@progress/kendo-angular-grid';
import {UserService} from '../services/user.service';
import {VkRepostModel, VkUser} from "./VkResponse";
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
  public userId: { FullName: '', UserId: '' };
  public search = '';
  public fromRepo = false;
  public timeout: number = 15;
  public loading = false;
  public selectedKeys: VkRepostModel[] = [];
  public state: State = {
    skip: 0,
    take: 100
  };

  constructor(private userService: UserService, private notificationService: NotificationService) {
  }

  ngOnInit() {
    this.userService.getUsers().subscribe(data => this.users = data.data);
  }

  handleSearch() {
    this.loading = true;
    this.userService
      .getReposts(this.userId != null ? this.userId.UserId : '', this.state, this.search, this.fromRepo)
      .subscribe(data => {
        this.data = data;
        this.loading = false;
      });
  }

  onSelectionChange(event: SelectionEvent) {
    if (event.selectedRows.length > 0) {
      event.selectedRows.forEach((item) => {
        this.selectedKeys.push({Id: item.dataItem.Id, Owner_Id: item.dataItem.Owner_Id});
      })
    }
    if (event.deselectedRows.length > 0) {
      event.deselectedRows.forEach((item) => {
        this.selectedKeys = this.selectedKeys.filter((l) => item.dataItem.Owner_Id != l.Owner_Id && item.dataItem.Id != l.Id);
      });
    }
  }

  onStateChange(state: DataStateChangeEvent) {
    this.state.skip = state.skip;
    this.state.take = state.take;
    this.handleSearch();
  }
  repostAll() {
    this.repost(this.selectedKeys, this.timeout, null);
  }

  repostOne(repost: VkRepostModel[], element: HTMLElement) {
    this.repost(repost, 0, () => {
      element.classList.remove('k-i-fav-outline');
      element.classList.add('text-danger');
      element.classList.add('k-i-fav');
    })
  }

  repost(repost: VkRepostModel[], timeout: number, callback: Function) {
    this.loading = true;
    this.userService.repost(repost, timeout).subscribe(z => {
      if (z) {
        this.notificationService.show(
          {
            content: 'Выполнено успешно',
            position: {vertical: 'bottom', horizontal: 'right'},
            type: {style: 'success', icon: true}
          });
        if (callback) {
          callback();
        }
        this.loading = false;
      }
    });
  }
}
