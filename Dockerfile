FROM mcr.microsoft.com/dotnet/sdk:10.0 as builder
WORKDIR /repo
COPY . .
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
RUN dotnet publish src/iTool.DiscordBot --configuration Release --output="/app"

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "iTool.DiscordBot.dll"]
