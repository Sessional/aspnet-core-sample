#!/usr/bin/env bash

set -euo pipefail
IFS=$'\n\t'

atlas schema apply \
  --url $DATABASE_URL \
  --to "file://./" \
  --var tenant_id=1 \
  --schema tenant_1,public
