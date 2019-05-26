FROM mcr.microsoft.com/dotnet/core/runtime:3.0.0-preview5-alpine3.9
WORKDIR /metrics
COPY metrics/bin/Release/netcoreapp3.0/alpine.3.9-x64/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet metrics.dll