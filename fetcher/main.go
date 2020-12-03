package main

import (
	"bytes"
	"context"
	"encoding/csv"
	"encoding/json"
	"fmt"
	"net/http"
	"os"
	"regexp"
	"strconv"
	"time"

	"github.com/jackc/pgx/v4"
)

const (
	token string = ""
)

type vkMessage struct {
	OwnerID   int
	MessageID int
	OwnerName string
	Category  string
	Text      string
}

type vkResponse struct {
	Response struct {
		Items []struct {
			ID      int    `json:"id"`
			OwnerID int    `json:"owner_id"`
			Text    string `json:"text"`
		} `json:"items"`
		Groups []struct {
			ID   int    `json:"id"`
			Name string `json:"name"`
		} `json:"groups"`
		Profiles []struct {
			ID        int    `json:"id"`
			FirstName string `json:"first_name"`
			LastName  string `json:"last_name"`
		} `json:"profiles"`
	} `json:"response"`
}

func main() {

	conn, err := pgx.Connect(context.Background(), "host=localhost port=5432 database=repost_ctx user=postgres password=password")
	if err != nil {
		fmt.Println(err)
		os.Exit(1)
	}

	defer conn.Close(context.Background())

	data := fetchData(conn)

	data = comb(data)

	toFile(data)

	_, e := conn.Exec(context.Background(), `update public."MessageVk" set "IsExported" = true where "IsExported" = false`)
	if e != nil {
		fmt.Println(e)
	}
}

func comb(data []vkMessage) []vkMessage {
	var buf []byte
	buffer := bytes.NewBuffer(buf)

	var response []vkMessage

	count := 0
	re := regexp.MustCompile(`\r?\n`)
	for i := 0; i < len(data); i++ {
		buffer.WriteString(fmt.Sprintf("%d_%d,", data[i].OwnerID, data[i].MessageID))

		count++
		if count == 100 || (i == len(data)-1 && count < 100) {
			query := fmt.Sprintf("https://api.vk.com/method/wall.getById?posts=%s&extended=1&access_token=%s&v=%s", buffer.String(), token, "5.124")
			resp, err := http.Get(query)

			if err == nil {
				defer resp.Body.Close()
				var decoded vkResponse

				err = json.NewDecoder(resp.Body).Decode(&decoded)

				if decoded.Response.Items != nil {
					for _, m := range decoded.Response.Items {
						for _, mv := range data {
							if mv.OwnerID == m.OwnerID && mv.MessageID == m.ID {
								var ownerName string
								for _, w := range decoded.Response.Groups {
									if w.ID == -mv.OwnerID {
										ownerName = w.Name
										break
									}
								}

								if ownerName == `` {
									for _, w := range decoded.Response.Profiles {
										if w.ID == mv.OwnerID {
											ownerName = w.FirstName + ` ` + w.LastName
											break
										}
									}
								}
								response = append(response, vkMessage{mv.OwnerID, mv.MessageID, re.ReplaceAllString(ownerName, " "), mv.Category, re.ReplaceAllString(m.Text, " ")})
							}
						}
					}
				}
			}

			time.Sleep(time.Second)
			buffer.Reset()
			count = 0
		}
	}

	return response
}

func fetchData(conn *pgx.Conn) []vkMessage {

	rows, _ := conn.Query(context.Background(), `SELECT 
	"MessageId", 
	"OwnerId", 
	"Title" from public."MessageVk" vk 
	join public."MessageCategory" ms on ms."Id" = vk."MessageCategoryId"
	where vk."IsExported" = false`)

	var data []vkMessage
	for rows.Next() {
		var messageID, ownerID int
		var title string
		_ = rows.Scan(&messageID, &ownerID, &title)
		data = append(data, vkMessage{ownerID, messageID, "", title, ""})
	}

	fmt.Println(fmt.Sprintf(`Fetched %d rows`, len(data)))

	return data
}

func toFile(data []vkMessage) {
	f, err := os.Open(`d:\mess.csv`)

	var rawData [][]string

	if err == nil {
		reader := csv.NewReader(f)
		lines, _ := reader.ReadAll()
		for _, h := range lines {
			rawData = append(rawData, h)
		}

	}

	defer f.Close()

	for _, d := range data {
		rawData = append(rawData, []string{strconv.Itoa(d.OwnerID), strconv.Itoa(d.MessageID), d.OwnerName, d.Category, d.Text})
	}

	f, err = os.Create(`d:\mess.csv`)

	writer := csv.NewWriter(f)
	if err = writer.WriteAll(rawData); err != nil {
		fmt.Println(err)
		os.Exit(1)
	}

	defer f.Close()
	defer f.Sync()
	defer writer.Flush()
}
