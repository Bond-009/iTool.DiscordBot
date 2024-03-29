using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace iTool.DiscordBot.Modules
{
    public sealed class TagsModule : ModuleBase<SocketCommandContext>, IDisposable
    {
        private readonly Settings _settings;
        private readonly TagDatabase _db;

        public TagsModule(Settings settings)
        {
            _settings = settings;

            _db = new TagDatabase();
            _db.Database.EnsureCreated();
        }

        [Command("tag create")]
        [Alias("createtag")]
        [Summary("Creates a new tag")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task CreateTag(string name, [Remainder]string text)
        {
            await _db.CreateTagAsync(Context, name, text, Context.Message.Attachments.FirstOrDefault()?.Url).ConfigureAwait(false);

            await Tag(name).ConfigureAwait(false);
        }

        [Command("tag")]
        [Summary("Searches for a tag")]
        [RequireContext(ContextType.Guild)]
        public async Task Tag(string name)
        {
            Tag tag = await _db.GetTagAsync(Context.Guild.Id, name);

            if (tag == null)
            {
                return;
            }

            await ReplyAsync(
                string.Empty,
                embed: new EmbedBuilder()
                {
                    Title = tag.Name,
                    Color = _settings.GetColor(),
                    Description = tag.Text,
                    ImageUrl = tag.Attachment
                }.Build()).ConfigureAwait(false);
        }

        [Command("tag delete")]
        [Alias("tag remove", "deletetag", "removetag")]
        [Summary("Deletes a tag")]
        [RequireContext(ContextType.Guild)]
        public async Task TagDelete(string name)
        {
            await _db.DeleteTagAsync(Context, name).ConfigureAwait(false);

            await ReplyAsync(
                string.Empty,
                embed: new EmbedBuilder()
                {
                    Title = $"Delete tag {name}",
                    Color = _settings.GetColor(),
                    Description = $"Successfully deleted tag {name}",
                }.Build()).ConfigureAwait(false);
        }

        [Command("tags")]
        [Alias("tag list", "tags list", "listtags")]
        [Summary("Lists all tags")]
        [RequireContext(ContextType.Guild)]
        public async Task TagList()
        {
            await ReplyAsync(
                string.Empty,
                embed: new EmbedBuilder()
                {
                    Title = $"Tags",
                    Color = _settings.GetColor(),
                    Description = string.Join(" ,", _db.GetTags(Context.Guild.Id)
                                                    .Select(x => x.Name)),
                }.Build()).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
