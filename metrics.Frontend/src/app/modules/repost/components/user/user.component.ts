import {Component, OnInit} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { VkUserService } from '../../services/vk-user.service';
import {finalize} from 'rxjs/operators';
import { VkMessage, VkRepostModel } from '../../models/VkMessageModel';
import { VkMessageService } from '../../services/vk-message.service';
import { PageEvent} from '@angular/material/paginator';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';
import VkUserModel from '../../models/VkUserModel';
import {Message} from '../../models/Message';
import {MessageCategoryService} from '../../services/message-category.service';
import {DataSourceResponse} from '../../models/DataSourceResponse';
import {MatSelectChange} from '@angular/material/select';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {
  form: FormGroup;
  messages: VkMessage[] = [];
  loading = false;
  page = 0;
  pageSize = 80;
  total = 0;
  pageSizeOptions: Array<number> = [
    20,
    40,
    60,
    80,
    100,
    200
  ];
  timeout = 30;
  public users: VkUserModel[];
  public categories: Message[];

  constructor(
    private userService: VkUserService,
    private fb: FormBuilder,
    private vkMessageService: VkMessageService,
    private messageService: MessageCategoryService
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      search: '',
      user: ''
    });

    this.userService.getUsers().subscribe(r => {
      this.users = r;
    });

    this.messageService.getTypes(0, 30).subscribe((data: DataSourceResponse<Message>) => {
      this.categories = data.Data;
    });
  }

  onSubmit() {
    this.loading = true;
    const user = this.form.get('user').value;
    this.vkMessageService
      .get(
        this.page,
        this.pageSize,
        this.form.get('search').value,
        user ? (user as VkUserModel).Id.toString() : ''
      )
      .pipe(finalize(() => (this.loading = false)))
      .subscribe(data => {
        this.messages = data.Data;
        this.total = data.Total;
      });
  }

  fromUnixTime(value: number): string {
    return new Date(value * 1000).toLocaleString();
  }

  like(ownerId: number, id: number) {
    const message = this.messages.find(
      a => a.Owner_Id === ownerId && a.Id === id
    );
    if (message && !message.Likes.User_Likes) {
      this.vkMessageService.like(ownerId, id).subscribe(() => {
        message.Likes.User_Likes = true;
      });
    }
  }

  repost(ownerId: number, id: number) {
    const message = this.messages.find(
      a => a.Owner_Id === ownerId && a.Id === id
    );
    if (message && !message.Reposts.User_reposted) {
      this.vkMessageService
        .repost([new VkRepostModel(ownerId, id)])
        .subscribe(() => {
          message.Reposts.User_reposted = true;
        });
    }
  }

  images(vkMessage: VkMessage) {
    if (vkMessage.Attachments) {
      return vkMessage.Attachments.filter(
        img => img.Photo && img.Photo.Sizes[3]
      ).map(item => item.Photo.Sizes[3].Url);
    }

    return [];
  }

  get selectedMessages(): VkMessage[] {
    return this.messages.filter(x => x.IsSelected);
  }

  onChangePage(pageEvent: PageEvent) {
    this.pageSize = pageEvent.pageSize;
    this.page = pageEvent.pageIndex;
    this.onSubmit();
  }

  repostAll() {
    this.vkMessageService
      .repost(
        this.selectedMessages.map(x => new VkRepostModel(x.Owner_Id, x.Id)),
        this.timeout
      )
      .subscribe(() => {});
  }

  onSelect(ev: MatSlideToggleChange, message: VkMessage) {
    message.IsSelected =
      ev.checked &&
      !this.selectedMessages.some(
        f => f.Id === message.Id && f.Owner_Id === message.Owner_Id
      );
  }

  sliderChange(value) {
    this.timeout = value;
  }

  displayFn(user: VkUserModel): string {
    return user ? user.FullName : '';
  }

  clearUser(e: MouseEvent) {
    e.preventDefault();
    this.form.controls.user.setValue('');
  }

  setType(id: number, ownerId: number, ev: MatSelectChange) {
    this.vkMessageService.setType(id, ownerId, ev.value).subscribe(() => {
    })
  }

  onCustomPageChange() {
    this.onSubmit();
  }
}
