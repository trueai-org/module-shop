#!/bin/bash

# create network: 
# docker network create shop-net

# docker-compose -p shop-api down

docker-compose -p shop-api up -d --build