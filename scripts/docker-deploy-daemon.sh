#!/usr/bin/env bash
set -e

BUILD_NUMBER=$1

if [ -z "$BUILD_NUMBER" ]; then usage; fi

docker service rm prometheus-daemon

docker service create --name prometheus-daemon --network styx --env General__ApplicationVersion=$BUILD_NUMBER nebuchadnezzar:5000/prometheus/daemon:$BUILD_NUMBER
