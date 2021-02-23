import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';
import {finalize} from 'rxjs/operators';
import {VkMessage, VkRepostModel} from '../../models/VkMessageModel';
import {VkMessageService} from '../../services/vk-message.service';
import {MatSlideToggleChange} from '@angular/material/slide-toggle';
import {Message} from '../../models/Message';
import {MessageCategoryService} from '../../services/message-category.service';
import {DataSourceResponse} from '../../models/DataSourceResponse';
import {MatSelectChange} from '@angular/material/select';
import NotificationService from '../../../../services/notification.service';
import {Observable} from 'rxjs';

type RequestPayload = {
  ownerId: number;
  messageId: number;
};

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss'],
})
export class UserComponent implements OnInit {
  form: FormGroup;
  messages: VkMessage[] = [];
  loading = false;
  additionalLoading = false;
  page = 0;
  pageSize = 80;
  total = 0;
  public categories: Message[];

  constructor(
    private fb: FormBuilder,
    private vkMessageService: VkMessageService,
    private messageService: MessageCategoryService,
    private notificationService: NotificationService
  ) {
  }

  ngOnInit() {
    this.form = this.fb.group({
      search: '',
    });

    this.messageService
      .getTypes(0, 30)
      .subscribe((data: DataSourceResponse<Message>) => {
        this.categories = data.Data;
      });

    this.notificationService.hub.subscribe(hub => {
      hub.on('reposted', (args: RequestPayload) => {
        const idx = this.messages.findIndex(x => x.OwnerId === args.ownerId && x.Id === args.messageId);

        if (idx >= 0) {
          this.messages[idx].UserReposted = true;
          this.messages[idx].IsSelected = false;
        }
      });
    });
  }

  onSubmit() {
    this.page = 0;
    this.loading = true;
    this.getData()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe(x => {
        this.total = x.Total;
        this.messages = x.Data;
      });
  }

  getData(): Observable<DataSourceResponse<VkMessage>> {
    return this.vkMessageService
      .get(this.page, this.pageSize, this.form.get('search').value);
  }

  like(ownerId: number, id: number) {
    const message = this.messages.find(
      (a) => a.OwnerId === ownerId && a.Id === id
    );
    if (message) {
      this.vkMessageService.like(ownerId, id).subscribe(() => {
      });
    }
  }

  repost(ownerId: number, id: number) {
    const message = this.messages.find(
      (a) => a.OwnerId === ownerId && a.Id === id
    );
    if (message) {
      this.vkMessageService
        .repost([new VkRepostModel(ownerId, id)])
        .subscribe(() => {
        });
    }
  }

  get selectedMessages(): Array<VkMessage> {
    return this.messages.filter((x) => x.IsSelected);
  }

  repostAll() {
    this.vkMessageService
      .repost(
        this.selectedMessages.map((x) => new VkRepostModel(x.OwnerId, x.Id))
      )
      .subscribe(() => {
      });
  }

  onSelect(ev: MatSlideToggleChange, message: VkMessage) {
    message.IsSelected =
      ev.checked &&
      !this.selectedMessages.some(
        (f) => f.Id === message.Id && f.OwnerId === message.OwnerId
      );
  }

  toDate(date: string): string {
    return new Date(date).toLocaleString();
  }

  setType(id: number, ownerId: number, ev: MatSelectChange) {
    this.vkMessageService.setType(id, ownerId, ev.value).subscribe(() => {
    });
  }

  track(idx: number, item: VkMessage) {
    return `${item.FromId}_${item.Id}`;
  }

  trimText(text: string, count: number): string {
    if (text.length <= count) {
      return text;
    }
    return text.slice(0, count) + '...';
  }

  get hasMessages(): boolean {
    return this.messages.length < this.total;
  }

  nextPage() {
    this.page++;
    this.additionalLoading = true;
    this.getData()
      .pipe(finalize(() => (this.additionalLoading = false)))
      .subscribe(x => {
        this.messages.push(...x.Data);
      });
  }
}
