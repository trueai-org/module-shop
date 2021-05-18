#!/bin/bash

# create network
# docker network create demo-shop-net

# docker-compose -f docker-compose.demo.api.yml -p demo-shop-api down

docker-compose -f docker-compose.demo.api.yml -p demo-shop-api up -d --build