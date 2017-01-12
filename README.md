# iTool.DiscordBot
[![travis-ci Build Status](https://api.travis-ci.org/Bond-009/iTool.DiscordBot.svg?branch=master)](https://travis-ci.org/Bond-009/iTool.DiscordBot)

Just a Discord bot

##Commands

| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| help | | | Returns all the enabled commands |
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

##Random commands
| Command | Aliases | Permission | Description |
| ------- | ------- | ---------- | ----------- |
| cat | | | Returns a random cat image |
