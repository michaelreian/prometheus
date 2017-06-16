#!/usr/bin/env bash
set -e

COMMAND=${1:-default}

pushd ./src/Prometheus.Core

if [ "$COMMAND" = "upgrade" ]; then
  name=DatabaseMigrations$(date +%s)
  dotnet ef migrations add $name --startup-project ../Prometheus.Api/
fi

dotnet ef database update --startup-project ../Prometheus.Api/

popd
