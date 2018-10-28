export class VkResponse<T> {
  public response: VkResponseItems<T>;
}

export class VkResponseItems<T> {
  public count: number;
  public Items: T;
  public groups: VkGroup[];
}

export class VkGroup {
  public id: number;
  public name: string;
}

export class VkMessage {
  public id: number;
  public owner_id: number;
  public from_id: number;
  public date: number;
  public text: string;
  public post_type: PostType;
  public copy_History: VkMessage[];
  public attachments: MessageAttachment[];
  public reposts: MessageReposts;
}

export class MessageReposts {
  public user_reposted: boolean;
  public count: number;
}


export enum PostType {
  post = 0
}
export enum MessageAttachmentType {
  photo = 0,

  audio = 1,

  video = 2,

  link = 3,

  poll = 4,

  page = 5,

  album = 6,

  doc = 7,

  posted_photo = 8,

  graffiti = 9,

  note = 10,

  app = 11,

  photos_list = 12,

  market = 13,

  market_album = 14,

  sticker = 15,

  pretty_cards = 16
}


export class MessageAttachment {
  public type: MessageAttachmentType;
  public photo: AttachmentPhoto;
}
export class AttachmentPhoto {
  public id: number;
  public sizes: PhotoSize[];
}
export class PhotoSize {
  public width: number;
  public height: number;
  public type: PhotoSizeType;
  public url: string;
}
export enum PhotoSizeType {
  m = 0,

  o = 1,

  p = 2,

  q = 3,

  r = 4,

  s = 5,

  x = 6,

  y = 7,

  z = 8,

  w = 9
}

export class VkRepostModel {
  owner_id: number;
  id: number;

  constructor(owner_id: number, id: number) {
    this.owner_id = owner_id;
    this.id = id;
  }
}

export class DataSourceResponseModel {
  data: any;
  total: number;
}
