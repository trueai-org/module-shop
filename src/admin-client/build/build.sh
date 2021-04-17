#!/bin/bash

imgname="shop-web"
imgport=6002
imgversion="1.0.0"

docker stop $imgname
docker rm $imgname
docker rmi $imgname:$imgname

docker build -t $imgname:$imgversion .

docker run -d --name $imgname -p $imgport:80 --restart=always $imgname:$imgversion

docker logs $imgname