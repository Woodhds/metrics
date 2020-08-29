import { Component, OnInit } from "@angular/core";
import { UserMessage } from "../models/UserMessage";
import { UserMessageService } from "../services/user-message.service";
import { PageEvent } from "@angular/material/paginator";

@Component({
  selector: "app-running",
  templateUrl: "./running.component.html",
  styleUrls: ["./running.component.scss"],
})
export class RunningComponent implements OnInit {
  displayColumns: ["Id", "Text"];
  pageSizes: [50, 100];
  total: number = 0;
  pageSize: number = 50;
  page: number = 0;
  data: UserMessage[];

  constructor(private userMessageService: UserMessageService) {}

  ngOnInit(): void {
    this.userMessageService.get(this.page, this.pageSize).subscribe((data) => {
      this.data = data.Data;
      this.total = data.Total;
    });
  }

  private async fetch() {
    const { Total, Data } = await this.userMessageService
      .get(this.page, this.pageSize)
      .toPromise();
    this.total = Total;
    this.data = Data;
  }

  async onChangePage({ pageIndex, pageSize }: PageEvent) {
    this.pageSize = pageSize;
    this.page = pageIndex;
    await this.fetch();
  }
}
