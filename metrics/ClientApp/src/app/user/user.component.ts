import { Component, OnInit, ViewEncapsulation, ChangeDetectionStrategy } from '@angular/core';
import {State} from '@progress/kendo-data-query';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { UserService } from '../services/user.service';
import { MessageAttachmentType } from './VkResponse';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class UserComponent implements OnInit {
  public data: GridDataResult;
  public userId  = '';
  public search = '';
  public attachmentType: MessageAttachmentType.photo;
  public loading = false;
  public state: State = {
    skip: 0,
    take: 100
  };
  constructor(private userService: UserService) {}

  ngOnInit() {}

  handleSearch() {
    this.loading = true;
    this.userService
      .getReposts(this.userId, this.state, this.search)
      .subscribe(data => {
        this.data = data;
        this.loading = false;
      });
  }

  repost(owner_id: number, id: number, element: HTMLElement) {
    this.userService.repost(owner_id, id);
    element.classList.remove('k-i-fav-outline');
    element.classList.add('text-danger');
    element.classList.add('k-i-fav');
  }
}
