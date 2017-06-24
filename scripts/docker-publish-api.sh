#!/usr/bin/env bash

BUILD_NUMBER=$1

if [ -z "$BUILD_NUMBER" ]; then usage; fi

DOCKER_IMAGE_NAME=nebuchadnezzar:5000/prometheus/api
TAG=$DOCKER_IMAGE_NAME:$BUILD_NUMBER

docker build -t $TAG ./artifacts/api

docker push $TAG
