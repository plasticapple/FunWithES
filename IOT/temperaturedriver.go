package main

import (
	"bufio"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"os"
	"strconv"
	"strings"
	"time"

	mqtt "github.com/eclipse/paho.mqtt.golang"
)

type Message struct {
	Name        string
	Temperature float64
	Time        int64
}

func main() {
	fmt.Println("hello")
	opts := mqtt.NewClientOptions().AddBroker("tcp://10.185.20.156:1883").SetClientID("SensorDriver1")
	opts.SetUsername("unisondriver")
	opts.SetPassword("unisondriver")
	client := mqtt.NewClient(opts)
	token := client.Connect()

	if token.Wait(); token.Wait() && token.Error() != nil {
		panic(token.Error())
	}

	for x := range time.Tick(50 * time.Millisecond) {
		time.Sleep(2 * time.Second)
		publishTemperature(x, client)
	}

	if token := client.Unsubscribe("/hardware/tempsensors"); token.Wait() && token.Error() != nil {
		fmt.Println(token.Error())
	}

	client.Disconnect(250)
}

func publishTemperature(t time.Time, client mqtt.Client) {

	//rand.Seed(time.Now().UTC().UnixNano())

	var temp = readTempFromFile()

	for key, value := range temp {
		fmt.Println(key, " ", value)

		m := Message{key, value, time.Now().UnixNano() / int64(time.Millisecond)}
		b, err := json.Marshal(m)
		if err != nil {
			panic(err)
		}

		if token := client.Publish("/hardware/tempsensors", 1, false, b); token.Wait() && token.Error() != nil {
			fmt.Println(token.Error())
		}

	}
}

func readTempFromFile() map[string]float64 {
	files, err := ioutil.ReadDir("/sys/bus/w1/devices")
	if err != nil {
		panic(err)
	}
	var tempresults = make(map[string]float64)

	var listofsensors []string

	for _, f := range files {
		if strings.HasPrefix(f.Name(), "28-") {
			listofsensors = append(listofsensors, f.Name())
		}
	}

	for _, sensorname := range listofsensors {
		file, err := os.Open("/sys/bus/w1/devices/" + sensorname + "/w1_slave")
		if err != nil {
			panic(err)
		}
		defer file.Close()
		scanner := bufio.NewScanner(file)

		// https://golang.org/pkg/bufio/#Scanner.Scan
		for scanner.Scan() {
			var text = scanner.Text()

			var index = strings.Index(text, "t=")
			if index != -1 {
				i, err := strconv.Atoi(text[index+2:])
				if err == nil {
					tempresults[sensorname] = (float64(i) / 1000)
				}
			}
		}
		if err := scanner.Err(); err != nil {
			panic(err)
		}

	}
	return tempresults

}
