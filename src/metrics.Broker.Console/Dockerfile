FROM mcr.microsoft.com/dotnet/core/sdk:3.1
ARG workdir="/src"

# Copy source code
COPY . $workdir

# Restore dependencies
WORKDIR $workdir
RUN dotnet restore ./metrics.Broker.Console/metrics.Broker.Console.csproj

# Publish project
RUN dotnet publish ./metrics.Broker.Console/metrics.Broker.Console.csproj --no-restore -c Release -o /app


# Create docker image from published project
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app

# Disable telemetry
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Don't forget to expose a port for the application
COPY --from=0 /app .
ENTRYPOINT ["dotnet", "metrics.Broker.Console.dll"]
