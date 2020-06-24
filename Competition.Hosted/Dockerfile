FROM mcr.microsoft.com/dotnet/core/sdk:3.1
ARG workdir="/src"

# Copy source code
COPY . $workdir

# Restore dependencies
WORKDIR $workdir
RUN dotnet restore ./Competition.Hosted/Competition.Hosted.csproj

# Publish project
RUN dotnet publish ./Competition.Hosted/Competition.Hosted.csproj --no-restore -c Release -o /app


# Create docker image from published project
# ------------------------------------------
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app

# Disable telemetry
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV CacheOptions__Configuration="redis-master:6379,password=password"
ENV ConsulConfig__Address="consul-consul-server:8500"
ENV AmqpOptions__Host="rabbitmq"
ENV ElasticOptions__Host="http://elasticsearch-master:9200"

# Don't forget to expose a port for the application
COPY --from=0 /app .
ENTRYPOINT ["dotnet", "Competition.Hosted.dll"]
