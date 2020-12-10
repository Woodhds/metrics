package main

import (
	"bytes"
	"context"
	"crypto/tls"
	"encoding/json"
	"fmt"
	"hash/fnv"
	"net/http"
	"os"
	"strconv"
	"sync"
	"time"
	message "user-fetcher/message"

	"github.com/elastic/go-elasticsearch/v7"
	"github.com/elastic/go-elasticsearch/v7/esutil"

	"github.com/jackc/pgx/v4"
)

var token string
var version string
var count int
var pg string
var es string

func main() {
	token = os.Getenv("ACCESS_TOKEN")
	version = os.Getenv("VERSION")
	count, _ = strconv.Atoi(os.Getenv("COUNT"))
	pg = os.Getenv("PG")
	es = os.Getenv("ES")

	conn, err := pgx.Connect(context.Background(), pg)

	if err != nil {
		fmt.Printf(err.Error())
		return
	}

	defer conn.Close(context.Background())

	rows, _ := conn.Query(context.Background(), `select "Id" from public."VkUserModel"`)

	var ids []int
	for rows.Next() {
		var id int
		err = rows.Scan(&id)
		if err == nil {
			ids = append(ids, id)
		}
	}

	var mutex sync.Mutex
	var wg sync.WaitGroup
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

	for _, id := range ids {
		for i := 1; i <= 4; i++ {
			wg.Add(1)
			go getMessages(&mutex, bulkIndexer, httpClient, &wg, id, i)
			if err != nil {
				fmt.Println(err)
			}
		}
	}
	defer bulkIndexer.Close(context.Background())
	wg.Wait()
}

func getMessages(mutex *sync.Mutex, bulkIndexer esutil.BulkIndexer, httpClient *http.Client, wg *sync.WaitGroup, id int, page int) {
	defer wg.Done()
	mutex.Lock()

	url := fmt.Sprintf(`https://api.vk.com/method/wall.get?owner_id=%d&offset=%d&filter=owner&count=%d&extended=1&access_token=%s&v=%s`,
		id, (page-1)*count, count, token, version)

	resp, e := httpClient.Get(url)

	if e != nil {
		fmt.Println(e)
		return
	}

	if resp == nil {
		fmt.Println("RESPONSE NULL")
		return
	}

	defer resp.Body.Close()

	var data message.VkWallResponse
	err := json.NewDecoder(resp.Body).Decode(&data)

	if err != nil {
		fmt.Println(err)
	}

	time.Sleep(time.Millisecond * 500)

	var yt bytes.Buffer
	for _, dataItem := range data.Response.Items {
		if len(dataItem.CopyHistory) > 0 {
			yt.WriteString(fmt.Sprintf("%d_%d,", dataItem.CopyHistory[0].OwnerID, dataItem.CopyHistory[0].ID))
		}
	}

	url = fmt.Sprintf(`https://api.vk.com/method/wall.getById?posts=%s&extended=1&access_token=%s&v=%s`, yt.String(), token, version)

	resp, e = httpClient.Get(url)

	var posts message.VkResponse
	e = json.NewDecoder(resp.Body).Decode(&posts)
	if e != nil {
		fmt.Println(e)
	}

	time.Sleep(time.Millisecond * 500)
	mutex.Unlock()

	var c int
	var m *message.VkMessageModel
	for _, post := range posts.Response.Items {

		m = vkMessageModel(post, id, posts.Response.Groups)

		c = c + 1
		reader := esutil.NewJSONReader(m)
		bulkIndexer.Add(context.Background(), esutil.BulkIndexerItem{
			DocumentID: fmt.Sprintf("%d_%d", m.OwnerId, m.ID),
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

func hash(ownerID, id int) int {
	hash := fnv.New32()
	hash.Write([]byte(fmt.Sprintf("%d_%d", ownerID, id)))

	return int(hash.Sum32())
}

func vkMessageModel(post *message.VkMessage, id int, groups []*message.VkGroup) *message.VkMessageModel {
	model := &message.VkMessageModel{
		ID:           post.ID,
		FromID:       post.FromID,
		Date:         post.Date,
		Images:       []string{},
		Identifier:   hash(post.OwnerID, post.ID),
		LikesCount:   post.Likes.Count,
		Owner:        "",
		OwnerId:      post.OwnerID,
		RepostedFrom: id,
		RepostsCount: post.Reposts.Count,
		Text:         post.Text,
		UserReposted: post.Reposts.UserReposted,
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
