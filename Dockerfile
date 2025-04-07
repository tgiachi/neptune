# Base image for the final container
FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS base
WORKDIR /app

# Install curl for healthcheck
RUN apk add --no-cache curl

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /src
COPY ["src/Neptune.Rest.Server/Neptune.Rest.Server.csproj", "src/Neptune.Rest.Server/"]
RUN dotnet restore "src/Neptune.Rest.Server/Neptune.Rest.Server.csproj"
COPY . .
WORKDIR "/src/src/Neptune.Rest.Server"
RUN dotnet build "Neptune.Rest.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build -a $TARGETARCH

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
RUN dotnet publish "Neptune.Rest.Server.csproj" -c $BUILD_CONFIGURATION  -o /app/publish \
-a $TARGETARCH \
-p:PublishSingleFile=true \
-p:PublishReadyToRun=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set non-root user for better security
# Creating user inside container rather than using $APP_UID since Alpine uses different user management
RUN adduser -D -h /app neptune && \
    chown -R neptune:neptune /app


# Create directories for data persistence
RUN mkdir -p /app/data /app/logs /app/scripts && \
    chown -R neptune:neptune /app/data /app/logs /app/scripts


# Health check using the environment variable for the web port
# Default to port 20001 if not set
HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:${NEPTUNE_WEB_PORT:-20001}/api/v1/status || exit 1

USER neptune

ENTRYPOINT ["./Neptune.Rest.Server"]

