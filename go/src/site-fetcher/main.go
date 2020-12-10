package main

import (
	"fmt"
	"net/http"
	"net/url"
	"strconv"
	"strings"
	"sync"

	"github.com/PuerkitoBio/goquery"
)

func main() {
	var wg sync.WaitGroup

	if err != nil {
		panic(0)
	}

	for i := 1; i <= 10; i++ {
		wg.Add(1)
		go fetch(&wg, i)
	}

	wg.Wait()
}

func fetch(wg *sync.WaitGroup, page int) {
	defer wg.Done()

	res, _ := http.PostForm("https://wingri.ru/main/getPosts",
		url.Values{
			"page_num": []string{strconv.Itoa(page)},
			"our":      []string{},
			"city_id":  []string{strconv.Itoa(97)},
		},
	)
	doc, _ := goquery.NewDocumentFromReader(res.Body)
	var data []*pb.VkMessage
	doc.Find(".grid-item .post_container .post_footer a").Each(func(i int, selection *goquery.Selection) {
		attr, exists := selection.Attr("href")
		if exists {
			arr := strings.Split(strings.Replace(attr, "https://vk.com/wall", "", 1), "_")
			owner, _ := strconv.Atoi(arr[0])
			id, _ := strconv.Atoi(arr[1])
			fmt.Printf("%d_%d\r\n", owner, id)
			data = append(data, &pb.VkMessage{ OwnerId: int32(owner), Id: int32(id) })
		}
	})
}
