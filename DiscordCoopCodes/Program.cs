using Discord;
using Discord.WebSocket;
using DiscordCoopCodes;
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
            switch(message.Content) {
                case "!newcode":
                    var words = new Words();
                    var code = words.GetRandomWord() + words.GetRandomWord() + words.GetRandomNumber();
                    await message.Channel.SendMessageAsync(code);
                    break;
            }
            if (message.Content == "!ping") {
                await message.Channel.SendMessageAsync("Pong!");
            }
        }
    }
}
