#!/usr/bin/env bash
set -e

OUTPUT=$(pwd)/artifacts/proxy

if [ -d $OUTPUT ]
then
    rm -rf $OUTPUT
fi

mkdir -p $OUTPUT

pushd ./src/Prometheus.Proxy

cp Dockerfile $OUTPUT
cp nginx.conf $OUTPUT

popd
