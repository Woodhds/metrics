FROM mcr.microsoft.com/dotnet/core/sdk:5.0
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
FROM mcr.microsoft.com/dotnet/core/aspnet:5.0

WORKDIR /app

# Disable telemetry
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Don't forget to expose a port for the application
COPY --from=0 /app .
ENTRYPOINT ["dotnet", "Competition.Hosted.dll"]
