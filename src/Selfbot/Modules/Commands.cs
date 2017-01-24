using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using System.Globalization;
using Selfbot;
using Selfbot.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Selfbot.Roslyn;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Selfbot.Modules.Addons;

namespace Maya.Modules.Commands
{
    [Name("General")]
    public class GeneralCommands : SelfbotModule
    {
        [Command("quote")]
        [Summary("Quote user messages")]
        public async Task Quote([Required] params string[] message_ids)
        {
            List<string> text = new List<string>();
            List<IMessage> msgs = new List<IMessage>();
            IUser author = null;
            foreach (string ids in message_ids)
            {
                bool ok = true;
                ulong id;
                if (ulong.TryParse(ids, out id))
                {
                    ok = false;
                    IMessage m = await Context.Channel.GetMessageAsync(id);
                    if (m.Content.Length == 0)
                    {
                        await ReplyEmbedAsync($"The message '{m.Id}' doesn't have any text.");
                        return;
                    }
                    if (author == null)
                        author = m.Author;
                    else if (author != m.Author)
                    {
                        await ReplyEmbedAsync($"The message '{m.Id}' doesn't belong to the same user ({author.Username}).");
                        return;
                    }
                    msgs.Add(m);
                }
                else if(ok)
                    text.Add(ids);
            }
            if (msgs.Count == 0)
            {
                await ReplyEmbedAsync("No messages to quote.");
                return;
            }
            DateTime older, newer;
            older = newer = msgs.First().Timestamp.DateTime;
            foreach (IMessage m in msgs.Skip(1))
            {
                if (older.CompareTo(m.Timestamp.DateTime) > 0)
                    older = m.Timestamp.DateTime;
                else if (newer.CompareTo(m.Timestamp.DateTime) < 0)
                    newer = m.Timestamp.DateTime;
            }
            if ((newer - older).TotalMinutes > 10)
            {
                await ReplyEmbedAsync("The time between messages is too big (> 10 minutes).");
                return;
            }
            var ordered_msgs = msgs.OrderBy(x => x.Timestamp);
            EmbedBuilder eb = new EmbedBuilder();
            eb.Author = new EmbedAuthorBuilder().WithName($"{author.Username}#{author.Discriminator}").WithIconUrl(author.AvatarUrl);
            eb.Color = Utils.getRandomColor();
            eb.Description = String.Join("\n", ordered_msgs.Select(x => x.Content));
            eb.Timestamp = older;
            await OverwriteAsync(String.Join(" ", text), eb);
        }

        [Command("embed")]
        [Summary("Embed the message")]
        public async Task Embed([Required, Remainder] string message = null)
        {
            await OverwriteAsync(message);
        }

        [Command("ping")]
        public async Task Ping()
        {
            await ReplyEmbedAsync($"🏓 Pong! ``{(Context.Client as DiscordSocketClient).Latency}ms``");
        }
    }

    [Name("Admin")]
    public class AdminCommands : SelfbotModule
    {
        [Command("eval", RunMode = RunMode.Async)]
        public async Task Eval([Required, Remainder] string code = null)
        {
            using (Context.Channel.EnterTypingState())
            {
                try
                {
                    var references = new List<MetadataReference>();
                    var referencedAssemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
                    foreach (var referencedAssembly in referencedAssemblies)
                        references.Add(MetadataReference.CreateFromFile(Assembly.Load(referencedAssembly).Location));
                    var scriptoptions = ScriptOptions.Default.WithReferences(references);
                    Globals globals = new Globals { Context = Context, Guild = Context.Guild as SocketGuild };
                    object o = await CSharpScript.EvaluateAsync(@"using System;using System.Linq;using System.Threading.Tasks;using Discord.WebSocket;using Discord;" + @code, scriptoptions, globals);
                    if (o == null)
                        await ReplyEmbedAsync("Done!");
                    else
                        await ReplyEmbedAsync(new EmbedBuilder().WithTitle("Result:").WithDescription(o.ToString()));
                }
                catch (Exception e)
                {
                    await ReplyEmbedAsync(new EmbedBuilder().WithTitle("Error:").WithDescription($"{e.GetType().ToString()}: {e.Message}\nFrom: {e.Source}"));
                }
            }
        }

        [Command("clean")]
        public async Task Clean(int messages = 30)
        {
            var msgs = await Context.Channel.GetMessagesAsync(messages).Flatten();
            msgs = msgs.Where(x => x.Author.Id == Context.Client.CurrentUser.Id);
            await Context.Channel.DeleteMessagesAsync(msgs);
        }

        [Command("変化の術")]
        public async Task Henge([Required, Remainder] IGuildUser user = null)
        {
            MemoryStream imgStream = null;
            try
            {
                using (var http = new HttpClient())
                {
                    using (var sr = await http.GetStreamAsync(user.AvatarUrl))
                    {
                        imgStream = new MemoryStream();
                        await sr.CopyToAsync(imgStream);
                        imgStream.Position = 0;
                    }
                }
            }
            catch (Exception)
            {
                await ReplyEmbedAsync("Something went wrong while downloading the image.");
                return;
            }
            await Context.Client.CurrentUser.ModifyAsync(x => x.Avatar = new Image(imgStream));
            await (await Context.Guild.GetCurrentUserAsync()).ModifyAsync(x => x.Nickname = user.Nickname ?? user.Username);
            await ReplyEmbedAsync("変化の術!");
        }

        [Command("playing")]
        public async Task Playing([Remainder] string text = null)
        {
            await (Context.Client as DiscordSocketClient).SetGameAsync(text);
            await ReplyEmbedAsync("Done!");
        }
    }

    [Name("Help")]
    public class HelpCommand : SelfbotModule
    {
        [Command("help")]
        [Summary("Shows the help command")]
        public async Task Help([Remainder] string command = null)
        {
            await ReplyEmbedAsync(await Context.MainHandler.CommandHandler.HelpEmbedBuilder(Context, command));
        }
    }
}
