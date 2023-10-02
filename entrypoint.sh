#!/bin/sh
set -e

service ssh start

dotnet bin/Release/net7.0/Chat.gRPC.dll
