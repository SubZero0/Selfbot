using Discord;
using Discord.Commands;
using Selfbot.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Selfbot.Modules.Addons
{
    public class SelfbotModule : ModuleBase<SelfbotCommandContext>
    {
        protected async Task ReplyEmbedAsync(string embedText) => await ReplyEmbedAsync(new EmbedBuilder().WithDescription(embedText).Build());
        protected async Task ReplyEmbedAsync(EmbedBuilder embed) => await ReplyEmbedAsync(embed.Build());
        protected async Task ReplyEmbedAsync(Embed embed)
        {
            await Context.Message.ModifyAsync(x => x.Embed = embed);
        }

        protected async Task OverwriteAsync(string text, bool insideEmbed = true)
        {
            if (insideEmbed)
                await OverwriteAsync(null, new EmbedBuilder().WithDescription(text).Build());
            else
                await OverwriteAsync(text, null);
        }
        protected async Task OverwriteAsync(string text, EmbedBuilder embed) => await OverwriteAsync(text, embed.Build());
        protected async Task OverwriteAsync(EmbedBuilder embed) => await OverwriteAsync(null, embed.Build());
        protected async Task OverwriteAsync(Embed embed) => await OverwriteAsync(null, embed);
        protected async Task OverwriteAsync(string text, Embed embed)
        {
            await Context.Message.ModifyAsync(x =>
            {
                x.Content = text ?? "";
                x.Embed = embed;
            });
        }
    }
    public class SelfbotCommandContext : ICommandContext
    {
        public IDiscordClient Client { get; }
        public IGuild Guild { get; }
        public MainHandler MainHandler { get; }
        public IMessageChannel Channel { get; }
        public IUser User { get; }
        public IUserMessage Message { get; }

        public bool IsPrivate => Channel is IPrivateChannel;

        public SelfbotCommandContext(IDiscordClient client, MainHandler handler, IUserMessage msg)
        {
            Client = client;
            Guild = (msg.Channel as IGuildChannel)?.Guild;
            Channel = msg.Channel;
            User = msg.Author;
            Message = msg;
            MainHandler = handler;
        }
    }
}
