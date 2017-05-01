# iTool.DiscordBot

[![travis-ci Build Status](https://api.travis-ci.org/Bond-009/iTool.DiscordBot.svg?branch=master)](https://travis-ci.org/Bond-009/iTool.DiscordBot)

Just a Discord bot

## Commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| help | | | Returns the enabled commands in lists of 25 |
| cmdinfo | commandinfo, cmdinformation, commandinformation | | Returns info about the command |
| info | information | | Returns info about the bot |
| invite | | | Returns the OAuth2 Invite URL of the bot |
| leave | | GuildPermission.ManageGuild | Instructs the bot to leave this Guild |
| setgame | | **Trusted user** | Sets the bots game |
| ping | | | Gets the estimated round-trip latency, in milliseconds, to the gateway server |
| userinfo | | | Returns info about the user |

## Administration commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| delmsgs | | GuildPermission.ManageMessages | Deletes the messages |
| kick | | GuildPermission.KickMembers | Kicks the user |
| ban | | GuildPermission.BanMembers | Bans the user |

## Audio commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| join | | | Joins the voice channel |
| stop | | | Stops the audio playback and leaves the voice channel |
| play | | | Plays an audio files |

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

## CS:GO commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| csgostats | | | Returns the CS:GO stats of the player |
| csgolastmatch | | | Returns stats of the player's last CS:GO match |

## Dev commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| gc | collectgarbage | **Thrusted user** | Forces the GC to clean up resources |
| eval | cseval, csharp, evaluate | **Thrusted user** | Evaluates C# code |

## HOTS commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| hotsstats | | | Returns the HOTS stats of the player |

## Random commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| cat | | | Returns a random cat image |
| dog | | | Returns a random dog image |

## Steam commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| vanityurl | resolvevanityurl | | Returns the steamID64 of the player |
| steam | getplayersummaries, playersummaries | | Returns basic steam profile information |
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
