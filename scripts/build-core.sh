#!/usr/bin/env bash
set -e

OUTPUT=$(pwd)/artifacts/core

if [ -d $OUTPUT ]
then
    rm -rf $OUTPUT
fi

dotnet restore

dotnet pack ./src/Prometheus.Core/Prometheus.Core.csproj --output $OUTPUT
