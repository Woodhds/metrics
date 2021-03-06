package message

import (
	"fmt"
	"strconv"
	"time"
)

type Timestamp time.Time

type VkWallResponse struct {
	Response struct {
		Items []struct {
			CopyHistory []struct {
				OwnerID int `json:"owner_id"`
				ID      int `json:"id"`
			} `json:"copy_history"`
		} `json:"items"`
	} `json:"response"`
}

type VkResponse struct {
	Response struct {
		Items    []*VkMessage `json:"items"`
		Groups   []*VkGroup   `json:"groups"`
		Profiles []*VkProfile `json:"profiles"`
	} `json:"response"`
}

type VkMessage struct {
	Date        *Timestamp `json:"date"`
	OwnerID     int        `json:"owner_id"`
	ID          int        `json:"id"`
	Text        string     `json:"text"`
	FromID      int        `json:"from_id"`
	Reposts     *VkReposts `json:"reposts"`
	Likes       *VkLikes   `json:"likes"`
	Attachments []struct {
		Photo struct {
			Sizes []struct {
				Url  string `json:"url"`
				Type string `json:"type"`
			} `json:"sizes"`
		} `json:"photo"`
	} `json:"attachments"`
}

type VkGroup struct {
	ID   int    `json:"id"`
	Name string `json:"name"`
}

type VkReposts struct {
	Count int `json:"count"`
}

type VkLikes struct {
	Count int `json:"count"`
}

type VkProfile struct {
	ID        int    `json:"id"`
	FirstName string `json:"first_name"`
	LastName  string `json:"last_name"`
}

func (t *Timestamp) MarshalJSON() ([]byte, error) {
	stamp := fmt.Sprintf("\"%s\"", time.Time(*t).UTC().Format(time.RFC3339))
	return []byte(stamp), nil
}

func (t *Timestamp) UnmarshalJSON(b []byte) error {
	ts, err := strconv.Atoi(string(b))
	if err != nil {
		return err
	}

	*t = Timestamp(time.Unix(int64(ts), 0))

	return nil
}
