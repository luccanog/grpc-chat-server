#!/bin/sh
set -e

service ssh start

echo '## entrypoint.sh script executing ##'

dotnet bin/Release/net7.0/Chat.gRPC.dll
