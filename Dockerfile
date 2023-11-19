FROM mcr.microsoft.com/dotnet/sdk:8.0 as builder
WORKDIR /repo
COPY . .
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
RUN dotnet publish src/iTool.DiscordBot --configuration Release --output="/app"

FROM mcr.microsoft.com/dotnet/runtime:8.0
COPY --from=builder /app /app
WORKDIR /app
VOLUME /app/settings /app/data
ENTRYPOINT ["./iTool.DiscordBot"]
