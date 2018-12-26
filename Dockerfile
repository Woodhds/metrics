FROM microsoft/dotnet:2.1.6-runtime-alpine3.7
WORKDIR /metrics
COPY metrics/bin/Release/netcoreapp2.2/alpine.3.7-x64/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet metrics.dll