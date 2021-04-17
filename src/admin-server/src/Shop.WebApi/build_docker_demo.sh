#!/bin/bash

echo Linux Docker build Docker

project_name="Shop.WebApi"
port=9101
image_name="demo-shop"
image_version="1.0.0"

# image version
echo $image_version;

cd /home/docker/images

# 如果已经存在解压包
rm -rf publish

unzip $project_name.zip

cd publish

# stop container
docker stop $image_name

# remove container
docker rm $image_name

# remove image
docker rmi $image_name:$image_version

# build image tag
docker build -t $image_name:$image_version .

# run
docker run -p $port:80 --restart=always --name $image_name --volume /home/$image_name/user-content:/app/wwwroot/user-content/ --link demo-shop-redis -d $image_name:$image_version

docker logs $image_name

cd /home/docker/images

rm -f $project_name.zip
rm -f build_docker_demo.sh
rm -rf publish