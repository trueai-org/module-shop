#!/bin/bash

# create network
# docker network create demo-shop-net

docker-compose -f docker-compose.demo.env.yml -p demo-shop-env down

docker-compose -f docker-compose.demo.env.yml -p demo-shop-env up -d --build