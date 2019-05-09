#!/bin/sh
export version=$1
export key=$2
dotnet pack -c release
dotnet nuget push ./Library/bin/Release/FsLogging.$version.nupkg -s https://api.nuget.org/v3/index.json -k $key

