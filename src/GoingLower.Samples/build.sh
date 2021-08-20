#!/bin/sh
echo "DOTNET CORE"
dotnet clean -v q
dotnet build -c Release -r linux-x64
dotnet publish --no-build --self-contained -c Release -r linux-x64

# NAME="PerfStructSize"
#echo ""
#echo "MONO"
#mcs -debug -optimize -define:$NAME -out:./bin/$NAME-mcs $NAME.cs
