# Building

To build i-Tool.DiscordBot you need the [.Net Core SDK](https://github.com/dotnet/core/blob/master/release-notes/download-archives/1.1.1-download.md)

If you are using Unix you can build by running the `./build.sh` script.

On Windows you need to execute this command: `dotnet restore && dotnet build -c Release`

## Self-contained

You can build self-contained versions for Debian 8 x64 and Windows 10 x64

Debian 8 x64: `dotnet restore && dotnet build -c Release -r debian.8-x64`

Windows 10: `dotnet restore && dotnet build -c Release -r win10-x64`

If you want a self-contained app for another OS you can change the `RuntimeIdentifiers` in iToolDiscordBot.csproj

and run `dotnet restore && dotnet build -c Release -r <RuntimeIdentifier>`

## Docker

To create a Docker image you need to build the bot first with this command: `dotnet restore && dotnet publish -c Release -r debian.8-x64 -o ../../docker-build`
Than you create the image with: `docker build .`
