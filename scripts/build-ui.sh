#!/usr/bin/env bash
set -e

OUTPUT=$(pwd)/artifacts/ui

if [ -d $OUTPUT ]
then
    rm -rf $OUTPUT
fi

pushd ./src/Prometheus.UI

npm install

webpack --output-path $OUTPUT

popd
