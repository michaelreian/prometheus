#!/usr/bin/env bash
set -e

pushd ./src/Prometheus.Core

name=datareaver-$(date +%s)

dotnet ef migrations add $name --startup-project ../Prometheus.Api/
dotnet ef database update --startup-project ../Prometheus.Api/

popd
