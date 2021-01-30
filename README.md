# iTool.DiscordBot

![.NET][github-actions-badge] [![codecov][codecov-badge]][codecov-link] [![Discord][discord-badge]][discord-invite] 

i-Tool bot is a general purpose bot that has moderation commands, can check your CS:GO, Battlefield 3, 4, H and HOTS stats and a lot more!

## Commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| help | | | Returns all the enabled modules |
| cmdinfo | commandinfo | | Returns info about the command |
| info | information | | Returns info about the bot |
| invite | | | Returns the OAuth2 Invite URL of the bot |
| leave | | GuildPermission.ManageGuild | Instructs the bot to leave the guild |
| setgame | | **Trusted user** | Sets the bots game |
| ping | | | Returns the estimated round-trip latency, in milliseconds, to the gateway server |
| userinfo | user | | Returns info about the user |
| serverinfo | server, guild, guildinfo | | Returns info about the guild |

## Administration commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| delmsgs | purge, clean | GuildPermission.ManageMessages | Deletes the messages |
| kick | | GuildPermission.KickMembers | Kicks the user(s) |
| ban | | GuildPermission.BanMembers | Bans the user(s) |

## Audio commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| join | | | Joins the voice channel |
| stop | | | Stops the audio playback and leaves the voice channel |
| play | | | Plays an audio file |

## Bf3 commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| bf3stats | | | Returns the Battlefield 3 stats of the player |

## Bf4 commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| bf4stats | | | Returns the Battlefield 4 stats of the player |

## BfH commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| bfhstats | | | Returns the Battlefield Hardline stats of the player |

## Core commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| blacklist | | | Adds the user to the blacklist |
| rmblacklist | | | Removes the user from the blacklist |
| trust | | | Adds the user to the list of trusted users |
| untrust | | | Removes the user from the list of trusted users |

## CS:GO commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| csgostats | | | Returns the CS:GO stats of the player |
| csgolastmatch | | | Returns stats of the player's last CS:GO match |

## Dev commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| gc | collectgarbage | **Trusted user** | Forces the GC to clean up resources |
| eval | cseval, csharp, evaluate | **Trusted user** | Evaluates C# code |

## Guild commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| prefix | | | Returns the current prefix |
| prefix | setprefix, prefix set | | Sets the current prefix |

## HOTS commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| hotsstats | | | Returns the HOTS stats of the player |

## Steam commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| vanityurl | resolvevanityurl | | Returns the steamID64 of the player |
| steam | | | Returns basic steam profile information |
| playerbans | getplayerbans | | Returns Community, VAC, and Economy ban statuses for given players |
| steamprofile | | | Returns the URL to the steam profile of the user |

## Tag commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| tag create| createtag | GuildPermission.ManageMessages | Creates a new tag |
| tag | | | Searches for a tag |
| tag delete | tag remove, deletetag, removetag | | Deletes a tag |
| tag list | tags list, listtags | | Lists all tags |

## Weather commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| weather | | | Returns info about the weather |

[github-actions-badge]: https://github.com/Bond-009/iTool.DiscordBot/workflows/.NET/badge.svg
[codecov-badge]: https://codecov.io/gh/Bond-009/iTool.DiscordBot/branch/master/graph/badge.svg?token=QD4OQ4Wgh6
[codecov-link]: https://codecov.io/gh/Bond-009/iTool.DiscordBot
[discord-badge]: https://discordapp.com/api/guilds/261241776105455618/widget.png
[discord-invite]: https://discordapp.com/invite/thKXwJb
