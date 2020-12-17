package main

import (
	"bytes"
	"context"
	"crypto/tls"
	"encoding/json"
	"fmt"
	"net/http"
	"net/url"
	"os"
	"strconv"
	"strings"
	"sync"
	"time"

	"github.com/PuerkitoBio/goquery"
	"github.com/elastic/go-elasticsearch/v7"
	"github.com/elastic/go-elasticsearch/v7/esutil"
)

type vkMessage struct {
	OwnerID int
	ID      int
}

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

type VkMessageModel struct {
	ID           int        `json:"id"`
	FromID       int        `json:"fromId"`
	Date         *Timestamp `json:"date"`
	Images       []string   `json:"images"`
	Identifier   int64      `json:"identifier"`
	LikesCount   int        `json:"likesCount"`
	Owner        string     `json:"owner"`
	OwnerID      int        `json:"ownerId"`
	RepostedFrom int        `json:"repostedFrom"`
	RepostsCount int        `json:"repostsCount"`
	Text         string     `json:"text"`
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

var cityID string
var token string
var version string
var es string

func main() {
	cityID = os.Getenv("CITY_ID")
	token = os.Getenv("ACCESS_TOKEN")
	version = os.Getenv("VERSION")
	es = os.Getenv("ES")

	var wg sync.WaitGroup
	var mutex sync.Mutex

	esClient, e := elasticsearch.NewClient(elasticsearch.Config{
		Addresses: []string{es},
	})

	if e != nil {
		panic(e.Error())
	}

	bulkIndexer, _ := esutil.NewBulkIndexer(esutil.BulkIndexerConfig{
		Index:  "vk_message",
		Client: esClient,
		OnFlushStart: func(c context.Context) context.Context {
			fmt.Printf("Flush start: %s\n", time.Now().UTC().String())
			return c
		},
	})

	httpClient := &http.Client{Transport: &http.Transport{TLSClientConfig: &tls.Config{InsecureSkipVerify: true}}}

	for i := 1; i <= 10; i++ {
		wg.Add(1)
		go fetch(&wg, bulkIndexer, httpClient, &mutex, i)
	}

	wg.Wait()
}

func fetch(wg *sync.WaitGroup, bulkIndexer esutil.BulkIndexer, httpClient *http.Client, mutex *sync.Mutex, page int) {
	defer wg.Done()

	res, err := httpClient.PostForm("https://wingri.ru/main/getPosts",
		url.Values{
			"page_num": []string{strconv.Itoa(page)},
			"our":      []string{},
			"city_id":  []string{cityID},
		},
	)

	if err != nil {
		panic(err)
	}

	doc, _ := goquery.NewDocumentFromReader(res.Body)
	var yt bytes.Buffer
	doc.Find(".grid-item .post_container .post_footer a").Each(func(i int, selection *goquery.Selection) {
		attr, exists := selection.Attr("href")
		if exists {
			arr := strings.Split(strings.Replace(attr, "https://vk.com/wall", "", 1), "_")
			owner, _ := strconv.Atoi(arr[0])
			id, _ := strconv.Atoi(arr[1])
			yt.WriteString(fmt.Sprintf("%d_%d,", owner, id))
		}
	})
	url := fmt.Sprintf(`https://api.vk.com/method/wall.getById?posts=%s&extended=1&access_token=%s&v=%s`, yt.String(), token, version)

	mutex.Lock()

	resp, e := httpClient.Get(url)

	time.Sleep(time.Millisecond * 500)
	mutex.Unlock()

	if e != nil {
		fmt.Println(e)
	}

	var posts VkResponse
	e = json.NewDecoder(resp.Body).Decode(&posts)

	var c int
	var m *VkMessageModel
	for _, post := range posts.Response.Items {

		m = vkMessageModel(post, posts.Response.Groups)

		c = c + 1
		reader := esutil.NewJSONReader(m)
		bulkIndexer.Add(context.Background(), esutil.BulkIndexerItem{
			DocumentID: fmt.Sprintf("%d_%d", m.OwnerID, m.ID),
			Body:       reader,
			Index:      "vk_message",
			Action:     "index",
			OnFailure: func(c context.Context, bii esutil.BulkIndexerItem, biri esutil.BulkIndexerResponseItem, e error) {
				fmt.Println(e.Error())
			},
		})
	}

	fmt.Printf("Fetched: %d\n", c)

}

func vkMessageModel(post *VkMessage, groups []*VkGroup) *VkMessageModel {
	model := &VkMessageModel{
		ID:           post.ID,
		FromID:       post.FromID,
		Date:         post.Date,
		Images:       []string{},
		Identifier:   time.Now().UTC().Unix(),
		LikesCount:   post.Likes.Count,
		Owner:        "",
		OwnerID:      post.OwnerID,
		RepostedFrom: 0,
		RepostsCount: post.Reposts.Count,
		Text:         post.Text,
	}

	for _, i := range post.Attachments {
		if len(i.Photo.Sizes) > 2 {
			model.Images = append(model.Images, i.Photo.Sizes[3].Url)
		}
	}

	for _, g := range groups {
		if g.ID == -post.OwnerID {
			model.Owner = g.Name
		}
	}

	return model
}
