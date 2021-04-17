#!/bin/bash

docker-compose -p shop-admin-web down

docker-compose -p shop-admin-web up -d --build