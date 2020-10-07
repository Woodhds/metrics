import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { finalize } from "rxjs/operators";
import { VkMessage, VkRepostModel } from "../../models/VkMessageModel";
import { VkMessageService } from "../../services/vk-message.service";
import { PageEvent } from "@angular/material/paginator";
import { MatSlideToggleChange } from "@angular/material/slide-toggle";
import { Message } from "../../models/Message";
import { MessageCategoryService } from "../../services/message-category.service";
import { DataSourceResponse } from "../../models/DataSourceResponse";
import { MatSelectChange } from "@angular/material/select";

@Component({
  selector: "app-user",
  templateUrl: "./user.component.html",
  styleUrls: ["./user.component.scss"],
})
export class UserComponent implements OnInit {
  form: FormGroup;
  messages: VkMessage[] = [];
  loading = false;
  page = 0;
  pageSize = 80;
  total = 0;
  pageSizeOptions: Array<number> = [20, 40, 60, 80, 100, 200];
  public categories: Message[];

  constructor(
    private fb: FormBuilder,
    private vkMessageService: VkMessageService,
    private messageService: MessageCategoryService
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      search: "",
    });

    this.messageService
      .getTypes(0, 30)
      .subscribe((data: DataSourceResponse<Message>) => {
        this.categories = data.Data;
      });
  }

  onSubmit() {
    this.loading = true;
    this.vkMessageService
      .get(this.page, this.pageSize, this.form.get("search").value)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((data) => {
        this.messages = data.Data;
        this.total = data.Total;
      });
  }

  like(ownerId: number, id: number) {
    const message = this.messages.find(
      (a) => a.OwnerId === ownerId && a.Id === id
    );
    if (message) {
      this.vkMessageService.like(ownerId, id).subscribe(() => {});
    }
  }

  repost(ownerId: number, id: number) {
    const message = this.messages.find(
      (a) => a.OwnerId === ownerId && a.Id === id
    );
    if (message) {
      this.vkMessageService
        .repost([new VkRepostModel(ownerId, id)])
        .subscribe(() => {});
    }
  }

  get selectedMessages(): Array<VkMessage> {
    return this.messages.filter((x) => x.IsSelected);
  }

  onChangePage(pageEvent: PageEvent) {
    this.pageSize = pageEvent.pageSize;
    this.page = pageEvent.pageIndex;
    this.onSubmit();
  }

  repostAll() {
    this.vkMessageService
      .repost(
        this.selectedMessages.map((x) => new VkRepostModel(x.OwnerId, x.Id))
      )
      .subscribe(() => {});
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
    this.vkMessageService.setType(id, ownerId, ev.value).subscribe(() => {});
  }

  onCustomPageChange() {
    this.onSubmit();
  }

  track(idx: number, item: VkMessage) {
    return `${item.FromId}_${item.Id}`;
  }
}
