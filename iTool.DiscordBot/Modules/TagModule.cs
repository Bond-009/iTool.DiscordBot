using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class TagModule : ModuleBase
    {
        [Command("createtag")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task CreateTag(string name, [Remainder]string text = "")
        {
            if (Context.Guild == null) { await ReplyAsync("This command can only be ran in a server."); return; }

            string attachment = string.Empty;
            if (Context.Message.Attachments.FirstOrDefault() != null)
            {
                attachment = Context.Message.Attachments.First().Url;
            }
            if (TagManager.CreateTag(new Tag()
                {
                    Author = Context.User.Id,
                    Title = name.ToLower(),
                    Text = text,
                    Attachment = attachment,
                }
                , Context.Guild.Id))
            {
                await ReplyAsync(":thumbsup:");
            }
            else
            {
                await ReplyAsync(":thumbsdown:");
            }
            
        }

        [Command("tag")]
        public async Task Tag(string name)
        {
            if (Context.Guild == null) { await ReplyAsync("This command can only be ran in a server."); return; }

            Tag tag = TagManager.GetTag(name, Context.Guild.Id);
            if (tag != null)
            {
                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = tag.Title,
                    Color = new Color(3, 144, 255),
                    Description = tag.Text,
                    ImageUrl = tag.Attachment
                });
            }
        }
    }
}
