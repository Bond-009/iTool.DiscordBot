# iTool.DiscordBot
[![travis-ci Build Status](https://api.travis-ci.org/Bond-009/iTool.DiscordBot.svg?branch=master)](https://travis-ci.org/Bond-009/iTool.DiscordBot)

Just a Discord bot

##Commands
| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| help | | | Returns the enabled commands in lists of 25 |
| cmdinfo | commandinfo, help | | Returns info about the command |
| info | | | Returns info about the bot |
| invite | | | Returns the OAuth2 Invite URL of the bot |
| leave | | GuildPermission.ManageGuild | Instructs the bot to leave this Guild |
| say | echo | GuildPermission.ManageGuild | Echos the provided input |
| setgame | | **Bot owner** | Sets the bot's game |
| userinfo | | | Returns info about the user |
| quit | exit, stop | **Bot owner** | Quits the bot |

##Administration commands
| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| delmsgs | | GuildPermission.ManageMessages | Deletes the messages |
| kick | | GuildPermission.KickMembers | Kicks the user |
| ban | | GuildPermission.BanMembers | Bans the user |

##CS:GO commands
| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| csgostats | | | Returns the CS:GO stats of the player |
| csgolastmatch | | | Returns stats of the player's last CS:GO match |

##HOTS commands
| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| hotsstats | | | Returns the HOTS stats of the player |

##Random commands
| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| cat | | | Returns a random cat image |
| dog | | | Returns a random dog image |

##Steam commands
| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| resolvevanityurl | | | Returns the steamID64 of the player |
| steamprofile | | | Returns the steamprofile of the user |

##Weather commands
| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| weather | | | Returns info about the weather |
