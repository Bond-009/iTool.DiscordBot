# Building

To build i-Tool.DiscordBot you need the [.Net Core SDK](https://github.com/dotnet/core/blob/master/release-notes/download-archives/1.1.1-download.md)

## Debug

To build in DEBUG on Unix run the `./build.sh` script

On Windows run: `dotnet restore && dotnet build`

## Release

To build in RELEASE run `dotnet restore && dotnet build -c Release`

## Self-contained

You can build self-contained versions for Windows 10 x64 and Ubuntu 16.10 x64

Windows 10: `dotnet restore && dotnet build -c Release -r win10-x64`

Ubuntu 16.10: `dotnet restore && dotnet build -c Release -r ubuntu.16.10-x64`

If you want a self-contained app for another OS you can change the `RuntimeIdentifiers` in iToolDiscordBot.csproj

and run `dotnet restore && dotnet build -c Release -r <RuntimeIdentifier>`

## Docker

To create a Docker image you need to build the bot first with this command: `dotnet restore && dotnet publish -c Release -r debian.8-x64 -o ../../docker-build`
Than you create the image with: `docker build .`
