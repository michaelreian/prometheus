#!/usr/bin/env bash
set -e

docker-compose down --remove-orphans

./scripts/build-ui.sh
./scripts/build-daemon.sh
./scripts/build-api.sh
./scripts/build-proxy.sh
./scripts/build-cli.sh

docker-compose build

docker-compose up
