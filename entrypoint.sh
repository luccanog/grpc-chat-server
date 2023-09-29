#!/bin/sh
set -e
service ssh start
dotnet Chat.gRPC.dll
