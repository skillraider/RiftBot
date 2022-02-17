docker build -t riftbot:latest .
docker stop riftbot
docker rm riftbot
docker run -d --restart=always --name riftbot riftbot:latest
