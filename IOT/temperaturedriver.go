package main

import (
	"bufio"
	"crypto/tls"
	"crypto/x509"
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

	cert, err := tls.LoadX509KeyPair("../democert/server.crt", "../democert/server.key")
	if err != nil {
		panic(err)
	}

	certpool := x509.NewCertPool()
	pemCerts, err := ioutil.ReadFile("../democert/ca.pem")
	if err == nil {
		certpool.AppendCertsFromPEM(pemCerts)
	}

	
	

	//log.Println(certtest)

	connOpts := &mqtt.ClientOptions{
		ClientID:             "RPI00000",
		CleanSession:         true,
		AutoReconnect:        true,
		MaxReconnectInterval: 1 * time.Second,
		//KeepAlive:            1 * time.Second,
		TLSConfig: tls.Config{
			// RootCAs = certs used to verify server cert.
		RootCAs: certpool,
		// ClientAuth = whether to request cert from server.
		// Since the server is set up for SSL, this happens
		// anyways.
		ClientAuth: tls.NoClientCert,
		// ClientCAs = certs used to validate client cert.
		ClientCAs: nil,
		// InsecureSkipVerify = verify that cert contents
		// match server. IP matches what is in cert etc.
		InsecureSkipVerify: true,
		// Certificates = list of certs client sends to server.
		Certificates: []tls.Certificate{cert},
			},
		
		Username:  "unisondriver",
		Password:  "unisondriver",
	}

	fmt.Println("hello")
	opts := connOpts.AddBroker("tcps://localhost:8883")
	//opts.SetUsername()
	//opts.SetPassword("unisondriver")
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
