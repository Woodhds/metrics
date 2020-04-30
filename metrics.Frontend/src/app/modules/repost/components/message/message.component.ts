import {AfterViewInit, Component, ViewChild} from '@angular/core';
import {MatPaginator} from '@angular/material/paginator';
import {MessageService} from '../../services/message.service';
import {of as observableOf} from 'rxjs';
import {catchError, map, startWith, switchMap} from 'rxjs/operators';
import {Message} from '../../models/Message';

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.scss']
})
export class MessageComponent implements AfterViewInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  resultsLength = 0;
  displayedColumns = ['Id', 'Title']
  data: Message[];

  constructor(private messageService: MessageService) {
  }

  ngAfterViewInit(): void {
    this.paginator.page
      .pipe(startWith({}),
        switchMap(() => {
          return this.messageService.getTypes(this.paginator.pageIndex, this.paginator.pageSize);
        }),
        map(data => {
          this.resultsLength = data.Total;
          return data.Data;
        }),
        catchError(() => {
          return observableOf([]);
        })
      ).subscribe(data => this.data = data);
  }

}
