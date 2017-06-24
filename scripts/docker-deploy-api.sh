#!/usr/bin/env bash
set -e

BUILD_NUMBER=$1

if [ -z "$BUILD_NUMBER" ]; then usage; fi

docker service rm prometheus-api

docker service create --name prometheus-api --publish 8088:8088 --network styx --env General__ApplicationVersion=$BUILD_NUMBER nebuchadnezzar:5000/prometheus/api:$BUILD_NUMBER
