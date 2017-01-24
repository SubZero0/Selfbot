using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Audio;
using System.IO;
using Selfbot.Controllers;

namespace Selfbot
{
    public class Selfbot
    {
        private DiscordSocketClient Discord;
        private MainHandler MainHandler;

        public async Task RunAsync()
        {
            Discord = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 50,
                DownloadUsersOnGuildAvailable = true
            });

            Discord.Log += (message) =>
            {
                Console.WriteLine($"{message.ToString()}");
                return Task.CompletedTask;
            };

            var map = new DependencyMap();
            map.Add(Discord);

            MainHandler = new MainHandler(Discord);
            await MainHandler.InitializeEarlyAsync(map);

            await Discord.LoginAsync(TokenType.User, "...");
            await Discord.ConnectAsync();
            Console.WriteLine($"Connected as {Discord.CurrentUser.Username}#{Discord.CurrentUser.Discriminator}!");

            await Task.Delay(-1);
        }
    }
}
