import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { VkUserService } from "../../services/vk-user.service";
import { finalize } from "rxjs/operators";
import { VkMessage, VkRepostModel } from "../../models/VkMessageModel";
import { VkMessageService } from "../../services/vk-message.service";
import { PageEvent } from "@angular/material/paginator";
import { MatSlideToggleChange } from "@angular/material/slide-toggle";
import VkUserModel from "../../models/VkUserModel";

@Component({
  selector: "app-user",
  templateUrl: "./user.component.html",
  styleUrls: ["./user.component.scss"]
})
export class UserComponent implements OnInit {
  form: FormGroup;
  messages: VkMessage[] = [];
  loading: boolean = false;
  page: number = 0;
  pageSize: number = 80;
  total: number = 0;
  pageSizeOptions: Array<number> = [
    20,
    40,
    60,
    80,
    100,
    200,
    400,
    600,
    800,
    1000
  ];
  timeout: number = 30;
  public users: VkUserModel[];

  constructor(
    private userService: VkUserService,
    private fb: FormBuilder,
    private vkMessageService: VkMessageService
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      search: "",
      user: ""
    });

    this.userService.getUsers().subscribe(r => {
      this.users = r;
    });
  }

  onSubmit() {
    this.loading = true;
    this.vkMessageService
      .get(
        this.page,
        this.pageSize,
        this.form.get("search").value,
        (<VkUserModel>this.form.get("user").value).Id.toString()
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

  like(owner_id: number, id: number) {
    let message = this.messages.find(
      a => a.Owner_Id === owner_id && a.Id === id
    );
    if (message && !message.Likes.User_Likes) {
      this.vkMessageService.like(owner_id, id).subscribe(() => {
        message.Likes.User_Likes = true;
      });
    }
  }

  repost(owner_id: number, id: number) {
    let message = this.messages.find(
      a => a.Owner_Id === owner_id && a.Id === id
    );
    if (message && !message.Reposts.User_reposted) {
      this.vkMessageService
        .repost([new VkRepostModel(owner_id, id)])
        .subscribe(() => {
          message.Reposts.User_reposted = true;
        });
    }
  }

  images(vkMessage: VkMessage) {
    if (vkMessage.Attachments) {
      return vkMessage.Attachments.filter(
        img => img.Photo && img.Photo.Sizes[4]
      ).map(item => item.Photo.Sizes[4].Url);
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
      .subscribe(() => {
        for (let message of this.messages) {
          message.IsSelected = false;
        }
      });
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
    return user ? user.FullName : "";
  }
}
