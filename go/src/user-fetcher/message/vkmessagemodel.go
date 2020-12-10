package message

type VkMessageModel struct {
	ID           int                 `json:"id"`
	FromID       int                 `json:"fromId"`
	Date         *Timestamp          `json:"date"`
	Images       []string            `json:"images"`
	Identifier   int                 `json:"identifier"`
	LikesCount   int                 `json:"likesCount"`
	Owner        string              `json:"owner"`
	OwnerId      int                 `json:"ownerId"`
	RepostedFrom int                 `json:"repostedFrom"`
	RepostsCount int                 `json:"repostsCount"`
	Text         string              `json:"text"`
	UserReposted *ConvertibleBoolean `json:"userReposted"`
}
