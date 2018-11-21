using Discord;
using Discord.WebSocket;
using DiscordCoopCodes;
using DiscordCoopCodes.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DiscordCoopCords {
    class Program {
        private static IConfigurationRoot Configuration;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() { 
            Configuration = new ConfigurationBuilder()
                .AddUserSecrets<Secrets>()
                .Build();

            var client = new DiscordSocketClient();

            client.Log += Log;
            client.MessageReceived += MessageReceived;

            await client.LoginAsync(TokenType.Bot, Configuration["Token"]);
            await client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage message) {
            Console.WriteLine($"Message: {message}");

            if (message.Content.StartsWith("!")) {
                var command = message.Content.Substring(1).Split(' ')[0].ToLower();
                switch(command) {
                    case "ping": await Ping.ExecuteAsync(message); break;
                    case "newcode": await NewCode.ExecuteAsync(message); break;
                }
            }
        }
    }
}
