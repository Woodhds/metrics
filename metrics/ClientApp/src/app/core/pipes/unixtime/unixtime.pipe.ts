import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'unixtime'
})
export class UnixtimePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    return new Date(value * 1000);
  }

}
