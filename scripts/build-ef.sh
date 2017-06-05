#!/usr/bin/env bash
set -e

pushd ./src/Prometheus.Core

name=DatabaseMigrations$(date +%s)

dotnet ef migrations add $name --startup-project ../Prometheus.Api/
dotnet ef database update --startup-project ../Prometheus.Api/

popd
