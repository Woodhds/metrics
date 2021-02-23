export class VkResponse<T> {
  public Response: VkResponseItems<T>;
}

export class VkResponseItems<T> {
  public Count: number;
  public Items: T;
  public Groups: VkGroup[];
}

export class VkGroup {
  public Id: number;
  public Name: string;
}

export class VkMessage {
  public Id: number;
  public OwnerId: number;
  public FromId: number;
  public LikesCount: number;
  public RepostsCount: number;
  public Date: string;
  public Owner: string;
  public Text: string;
  public Images: string[] = [];
  public IsSelected: boolean;
  public MessageCategoryId: number | null;
  public MessageCategoryPredict: string | null;
  public UserReposted: boolean;
}

export class VkRepostModel {
  OwnerId: number;
  Id: number;

  constructor(owner_id: number, id: number) {
    this.OwnerId = owner_id;
    this.Id = id;
  }
}

export class VkLike {
  public Count: number;
  public User_Likes: boolean;
}
