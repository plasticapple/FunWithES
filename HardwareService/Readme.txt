
docker run -d --name zookeeper --publish 2181:2181 zookeeper:3.4

docker run -d  --hostname localhost  --name kafka  --volume ./data:/data --volume ./logs:/logs --publish 9092:9092 --publish 7203:7203  --env KAFKA_ADVERTISED_HOST_NAME=10.185.20.152  --env ZOOKEEPER_IP=10.185.20.152 ches/kafka