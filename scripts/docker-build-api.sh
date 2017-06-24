#!/usr/bin/env bash
set -e

OUTPUT=./artifacts/api

if [ -d $OUTPUT ]
then
    rm -rf $OUTPUT
fi

docker build -t build-image -f ./scripts/Dockerfile.api.build .

docker create --name build-cont build-image

docker cp build-cont:/workspace/artifacts/api $OUTPUT
