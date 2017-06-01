#!/usr/bin/env bash
set -e

OUTPUT=$(pwd)/artifacts/daemon

if [ -d $OUTPUT ]
then
    rm -rf $OUTPUT
fi

dotnet restore

dotnet publish ./src/Prometheus.Daemon/Prometheus.Daemon.csproj --output $OUTPUT
