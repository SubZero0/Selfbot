using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Selfbot.Controllers
{
    public class MainHandler
    {
        public DiscordSocketClient Client;

        public CommandHandler CommandHandler { get; private set; }

        public readonly string Prefix = "self.";

        public MainHandler(DiscordSocketClient Discord)
        {
            Client = Discord;
            CommandHandler = new CommandHandler();
        }

        public async Task InitializeEarlyAsync(IDependencyMap map)
        {
            await CommandHandler.InitializeAsync(this, map);
        }
    }
}
