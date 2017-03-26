#FROM microsoft/dotnet
FROM tobond/dotnetsdk
COPY . .
RUN dotnet restore
RUN dotnet publish iTool.DiscordBot.sln -c Release -o ../../publish
RUN dotnet clean
RUN rm -r src/iTool.DiscordBot/bin
RUN rm -r src/iTool.DiscordBot/obj
ENTRYPOINT ["dotnet", "publish/iTool.DiscordBot.dll"]
