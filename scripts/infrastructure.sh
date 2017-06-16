#!/usr/bin/env bash
set -e

FILE=./docker-compose.infrastructure.yml

docker-compose --file $FILE down --remove-orphans

docker-compose --file $FILE build

docker-compose --file $FILE up
