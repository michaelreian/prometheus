#!/usr/bin/env bash
set -e

OUTPUT=./artifacts/api

if [ -d $OUTPUT ]
then
    rm -rf $OUTPUT
fi

mkdir -p $OUTPUT

docker build -t build-image -f ./scripts/Dockerfile.api.build .

BUILD_CONTAINER=$(docker create build-image)

docker cp $BUILD_CONTAINER:/workspace/artifacts/api $OUTPUT

docker rm $BUILD_CONTAINER
