#!/usr/bin/env bash
set -e

OUTPUT=$(pwd)/artifacts/ui
export BUILD_NUMBER=${1:-0}

if [ -d $OUTPUT ]
then
    rm -rf $OUTPUT
fi

pushd ./src/Prometheus.UI

npm install

webpack --output-path $OUTPUT --env.BUILD_NUMBER $BUILD_NUMBER

cp Dockerfile $OUTPUT
cp nginx.conf $OUTPUT

popd
