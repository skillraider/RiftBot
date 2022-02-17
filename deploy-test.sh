docker build -t riftbot:test .
docker stop riftbot-test
docker rm riftbot-test
docker run -d -e Environment=Test --net=riftbot --restart=always --name riftbot-test riftbot:test
