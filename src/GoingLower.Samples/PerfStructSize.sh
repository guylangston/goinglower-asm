#!/bin/bash
NAME="PerfStructSize"
echo "DOTNET CORE"
# The command below vvvv   requires the dotnet publish + 
# csc "-out:./bin/$NAME-csc" -debug -o -define:SINGLE_FILE "$NAME.cs"
dotnet publish --self-contained -c Release -r linux-x64

echo ""
echo "MONO"
mcs -debug -optimize -define:$NAME -out:./bin/$NAME-mcs $NAME.cs