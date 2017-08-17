# Building

To build i-Tool.DiscordBot you need the [.Net Core SDK](https://github.com/dotnet/core/blob/master/release-notes/download-archives/1.1.1-download.md)

If you are using Unix you can build by running the `./build.sh` script.

On Windows you need to execute this command: `dotnet restore && dotnet build -c Release`

## Self-contained

You can build a self-contained version for Windows 10 x64

`dotnet restore && dotnet build -c Release -r win10-x64`

If you want a self-contained app for another OS you can change the `RuntimeIdentifiers` in iToolDiscordBot.csproj

and run `dotnet restore && dotnet build -c Release -r <RuntimeIdentifier>`
