using Discord;
using Discord.Commands;
using LiteDB;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Tags : ModuleBase
    {
        Settings settings;

        public Tags(Settings settings) => this.settings = settings;

        [Command("tag create")]
        [Alias("createtag")]
        [Summary("Creates a new tag")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task CreateTag(string name, [Remainder]string text)
        {
            Directory.CreateDirectory(Common.GuildsDir + Path.DirectorySeparatorChar + Context.Guild.Id);

            using (LiteDatabase db = new LiteDatabase(Common.GuildsDir + Path.DirectorySeparatorChar + Context.Guild.Id + Path.DirectorySeparatorChar + "tags.db"))
            {
                LiteCollection<Tag> col = db.GetCollection<Tag>("tags");

                if (!col.EnsureIndex(x => x.Title, true))
                {
                    await ReplyAsync("", embed: new EmbedBuilder()
                    {
                        Title = "Failed to create tag",
                        Color = new Color((uint)settings.ErrorColor),
                        Description = "A tag with that name already exists",
                    });
                    return;
                }

                col.Insert(new Tag()
                {
                    Author = Context.User.Id,
                    Title = name.ToLower(),
                    Text = text,
                    Attachment = Context.Message.Attachments.FirstOrDefault()?.Url,
                });
            }

            await Tag(name);
        }

        [Command("tag")]
        [Summary("Searches for a tag")]
        [RequireContext(ContextType.Guild)]
        public async Task Tag(string name)
        {
            if (!File.Exists(Common.GuildsDir + Path.DirectorySeparatorChar + Context.Guild.Id + Path.DirectorySeparatorChar + "tags.db"))
            { return; }

            using (LiteDatabase db = new LiteDatabase(Common.GuildsDir + Path.DirectorySeparatorChar + Context.Guild.Id + Path.DirectorySeparatorChar + "tags.db"))
            {
                Tag tag = db.GetCollection<Tag>("tags").Find(x => x.Title == name).FirstOrDefault();

                if (tag == null)
                { return; }

                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = tag.Title,
                    Color = new Color((uint)settings.Color),
                    Description = tag.Text,
                    ImageUrl = tag.Attachment
                });
            }
        }

        [Command("tag delete")]
        [Alias("tag remove", "deletetag", "removetag")]
        [Summary("Deletes a tag")]
        [RequireContext(ContextType.Guild)]
        public async Task TagDelete(string name)
        {
            if (!File.Exists(Common.GuildsDir + Path.DirectorySeparatorChar + Context.Guild.Id + Path.DirectorySeparatorChar + "tags.db"))
            { return; }

            using (LiteDatabase db = new LiteDatabase(Common.GuildsDir + Path.DirectorySeparatorChar + Context.Guild.Id + Path.DirectorySeparatorChar + "tags.db"))
            {
                LiteCollection<Tag> col = db.GetCollection<Tag>("tags");
                int? id = col.Find(x => x.Title == name
                                && (x.Author == Context.User.Id || Context.User.Id == Context.Guild.OwnerId))
                            .FirstOrDefault()?.Id;

                if (id == null)
                { return; }

                col.Delete(id);
            }
            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Delete tag {name}",
                Color = new Color((uint)settings.Color),
                Description = $"Successfully deleted tag {name}",
            });
        }

        [Command("tag list")]
        [Alias("tags list", "listtags")]
        [Summary("Lists all tags")]
        [RequireContext(ContextType.Guild)]
        public async Task TagList()
        {
            if (!File.Exists(Common.GuildsDir + Path.DirectorySeparatorChar + Context.Guild.Id + Path.DirectorySeparatorChar + "tags.db"))
            { return; }

            using (LiteDatabase db = new LiteDatabase(Common.GuildsDir + Path.DirectorySeparatorChar + Context.Guild.Id + Path.DirectorySeparatorChar + "tags.db"))
            {
                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = $"List tags",
                    Color = new Color((uint)settings.Color),
                    Description = string.Join(" ,", db.GetCollection<Tag>("tags")
                                                    .FindAll()
                                                    .Select(x => x.Title)),
                });
            }
        }
    }
}
