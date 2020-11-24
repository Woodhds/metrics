import {AfterViewInit, Component, ViewChild} from '@angular/core';
import {MatPaginator} from '@angular/material/paginator';
import {MessageCategoryService} from '../../services/message-category.service';
import {of as observableOf} from 'rxjs';
import {catchError, map, startWith, switchMap} from 'rxjs/operators';
import {Message} from '../../models/Message';
import {MatDialog} from '@angular/material/dialog';
import {DialogComponent} from '../dialog/dialog.component';
import {MatSnackBar} from '@angular/material/snack-bar';
import {CdkDragDrop, moveItemInArray} from "@angular/cdk/drag-drop";
import {MatTable} from "@angular/material/table";

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.scss']
})
export class MessageComponent implements AfterViewInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatTable) table: MatTable<Message>;
  resultsLength = 0;
  displayedColumns = ['Id', 'Title', 'Color', 'Actions']
  data: Message[];
  isLoading = false;

  constructor(private messageService: MessageCategoryService, private dialog: MatDialog, private _snackbar: MatSnackBar) {
  }

  ngAfterViewInit(): void {
    this.fetch();
  }

  openDialog(message) {
    const dialogRef = this.dialog.open(DialogComponent, {
      data: message ? {...message} : new Message()
    })

    dialogRef.afterClosed().subscribe((data: Message) => {
      if (data) {
        this.messageService.save(data).subscribe(_ => {
          this._snackbar.open('Операция выполнена успешно')
          this.fetch();
        });
      }
    })
  }

  deleteMessage(id: number) {
    this.messageService.delete(id).subscribe(_ => {
      this._snackbar.open('Элемент удален')
      this.fetch()
    })
  }

  fetch() {
    this.paginator.page
      .pipe(startWith({}),
        switchMap(() => {
          this.isLoading = true;
          return this.messageService.getTypes(this.paginator.pageIndex, this.paginator.pageSize);
        }),
        map(data => {
          this.isLoading = false;
          this.resultsLength = data.Total;
          return data.Data;
        }),
        catchError(() => {
          this.isLoading = false;
          return observableOf([]);
        })
      ).subscribe(data => this.data = data);
  }

  dropTable(e: CdkDragDrop<Message[]>) {
    moveItemInArray(this.data, e.previousIndex, e.currentIndex);
    this.data.forEach((v, i) => v.SortOrder = i)
    this.messageService.saveList(this.data).subscribe(response => {
      response.forEach(v => {
        const idx = this.data.findIndex(a => a.Id === v.Id);

        if (idx >= 0) {
          this.data[idx].RowVersion = v.RowVersion;
        }
      })
    })
    this.table.renderRows();
  }
}
