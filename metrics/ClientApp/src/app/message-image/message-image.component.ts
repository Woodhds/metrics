import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { MessageAttachment, PhotoSizeType } from '../user/VkResponse';
import { of } from 'rxjs';

@Component({
  selector: 'app-message-image',
  templateUrl: './message-image.component.html',
  styleUrls: ['./message-image.component.scss']
})
export class MessageImageComponent implements OnInit, OnChanges {

  @Input() attachments: MessageAttachment[];
  public images: string[] = [];
  constructor() { }

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.attachments && this.attachments.length > 0) {
        for (let i = 0; i < this.attachments.length; i++) {
          const photo = this.attachments[i].photo;
          if (photo) {
          const size = photo.sizes.filter(z => z.type === PhotoSizeType.p);
          if (size.length) {
            this.images.push(size[0].url);
          }
        }
      }
    }
  }
}
