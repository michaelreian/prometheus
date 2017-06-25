#!/usr/bin/env bash
set -e

OUTPUT=./artifacts

if [ -d $OUTPUT ]
then
    rm -rf $OUTPUT
fi

mkdir -p $OUTPUT

docker build -t build-image -f ./scripts/Dockerfile.daemon.build .

BUILD_CONTAINER=$(docker create build-image)

docker cp $BUILD_CONTAINER:/workspace/artifacts/daemon $OUTPUT

docker rm $BUILD_CONTAINER
