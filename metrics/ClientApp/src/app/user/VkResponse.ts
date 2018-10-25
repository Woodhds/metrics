export class VkResponse<T>
{
  public Response: VkResponseItems<T>;
}

export class VkResponseItems<T>
{
  public Count: number;
  public Items: T;
  public Groups: VkGroup[];
}

export class VkGroup {
  public Id: number;
  public Name: string;
}

export class VkMessage {
  public id: number;
  public owner_id: number;
  public from_id: number;
  public date: number;
  public Text: string;
  public Post_type: PostType;
  public Copy_History: VkMessage[];
  public Attachments: MessageAttachment[];
  public Reposts: MessageReposts;
}

export class MessageReposts {
  public User_reposted: boolean;
  public Count: number;
}


export enum PostType {
  Post = 0
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
  public Type: MessageAttachmentType;
  public Photo: AttachmentPhoto;
}
export class AttachmentPhoto {
  public id: number;
  public Sizes: PhotoSize[];
}
export class PhotoSize {
  public Width: number;
  public Height: number;
  public Type: PhotoSizeType;
  public Url: string;
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
