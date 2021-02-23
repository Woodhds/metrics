import { Component, OnInit } from '@angular/core';
import { UserMessage } from '../models/UserMessage';
import { UserMessageService } from '../services/user-message.service';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-running',
  templateUrl: './running.component.html',
  styleUrls: ['./running.component.scss'],
})
export class RunningComponent implements OnInit {
  displayColumns = ['Id', 'DateStatus', 'Text'];
  pageSizes = [50, 100];
  total = 0;
  pageSize = 50;
  page = 0;
  data: Array<UserMessage> = [];

  constructor(private userMessageService: UserMessageService) {}

  ngOnInit(): void {
    this.userMessageService.get(this.page, this.pageSize).subscribe((data) => {
      this.data = data.Data;
      this.total = data.Total;
    });
  }

  private fetch() {
    this.userMessageService
      .get(this.page, this.pageSize)
      .subscribe(({Total, Data}) => {
        this.total = Total;
        this.data = Data;
      });

  }

  onChangePage({ pageIndex, pageSize }: PageEvent) {
    this.pageSize = pageSize;
    this.page = pageIndex;
    this.fetch();
  }
}
