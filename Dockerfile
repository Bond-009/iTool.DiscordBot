FROM microsoft/dotnet
COPY . .
RUN dotnet restore
RUN dotnet publish iTool.DiscordBot.sln -c Release -o ../../publish
RUN rm -r src/iTool.DiscordBot/bin
RUN rm -r src/iTool.DiscordBot/obj
ENTRYPOINT ["dotnet", "publish/iTool.DiscordBot.dll"]
