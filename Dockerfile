FROM microsoft/dotnet
COPY . .
RUN dotnet restore
RUN dotnet publish iTool.DiscordBot.sln -c Release
ENTRYPOINT ["dotnet", "src/iTool.DiscordBot/bin/Release/netcoreapp1.1/publish/iTool.DiscordBot.dll"]
