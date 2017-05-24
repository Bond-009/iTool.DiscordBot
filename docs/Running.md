# Running

When you run iTool.DiscordBot for the first time, it creates a settings folder with inside of it a settings.yaml file. Edit the settings.yaml file with your token and your preferred settings and start the bot again.

## Docker

When you run the container for the fist time it it creates a settings folder with inside of it a settings.yaml file. To edit the file copy it out of the container with: `docker cp <container>:publish/settings/settings.yaml settings.yaml`
Edit the settings file with your token and your preferred settings. Copy the settings file back into the container with: `docker cp settings.yaml <container>:publish/settings/settings.yaml` and start the container.
