#!/bin/sh
set -e
echo '## entrypoint.sh script executing ##'
service ssh start
dotnet bin/Release/net7.0/Chat.gRPC.dll
