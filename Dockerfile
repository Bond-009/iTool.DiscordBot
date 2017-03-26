#FROM microsoft/dotnet
FROM tobond/dotnetsdk
COPY . .
RUN dotnet restore
RUN dotnet publish iTool.DiscordBot.sln -c Release -r debian.8-x64 -o ../../publish
# Clearing local packages
RUN dotnet nuget locals all -c
# Uninstalling dotnet
RUN rm -f /usr/bin/dotnet
RUN rm -rf /opt/dotnet
# Removing soucre code
RUN rm -r src/
ENTRYPOINT ["publish/iTool.DiscordBot"]
