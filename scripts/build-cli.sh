#!/usr/bin/env bash
set -e

OUTPUT=$(pwd)/artifacts/cli

if [ -d $OUTPUT ]
then
    rm -rf $OUTPUT
fi

dotnet restore

dotnet publish ./src/Prometheus.Cli/Prometheus.Cli.csproj --output $OUTPUT
