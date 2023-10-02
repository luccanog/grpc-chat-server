#!/bin/sh
echo '## Entrypoint ##'

set -e

echo "Starting SSH ..."
/usr/sbin/sshd

echo "Starting Server ..."
dotnet bin/Release/net7.0/Chat.gRPC.dll
