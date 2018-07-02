
docker run -d --name zookeeper --publish 2181:2181 zookeeper:3.4

docker run -d  --hostname localhost  --name kafka  --volume ./data:/data --volume ./logs:/logs --publish 9092:9092 --publish 7203:7203  --env KAFKA_ADVERTISED_HOST_NAME=10.185.20.197  --env ZOOKEEPER_IP=10.185.20.197 ches/kafka

to see events in console use:

kafka-console-consumer --bootstrap-server localhost:9092 --topic HardwareService.domain.events.TemperatureSensorCreated  --from-beginning

delete topic is :

kafka-topics --zookeeper  localhost:2181 --delete --topic HardwareService.domain.events.TemperatureSensorCreated

attach to docker image:

docker exec -it <image> bash

kafka config is at \config\server.properties