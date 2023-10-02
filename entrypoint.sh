#!/bin/sh
echo '## entrypoint.sh script executing ##'

set -e

service ssh start


dotnet bin/Release/net7.0/Chat.gRPC.dll
