#!/usr/bin/env bash
set -e

OUTPUT=$(pwd)/artifacts/api

if [ -d $OUTPUT ]
then
    rm -rf $OUTPUT
fi

dotnet restore

dotnet publish ./src/Prometheus.Api/Prometheus.Api.csproj --output $OUTPUT

pushd ./src/Prometheus.Api

cp Dockerfile $OUTPUT

popd
