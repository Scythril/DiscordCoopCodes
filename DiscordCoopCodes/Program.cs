using Discord;
using Discord.WebSocket;
using DiscordCoopCodes;
using DiscordCoopCodes.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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
                var args = message.Content.Split(' ').Skip(1).ToArray();
                switch(command) {
                    case "ping": await Ping.ExecuteAsync(message); break;
                    case "newcode": await NewCode.ExecuteAsync(message); break;
                    case "contracttime": await TimeRemaining.ExecuteAsync(message, args); break;
                    case "help":
                        await message.Channel.SendMessageAsync($@"Available Commands: 
!ping (Tests to see if bot is listening) 
!newcode (Will give a new code for starting a contract),
!contracttime {{targetEggsShipped}} {{currentEggsShipped}} {{currentShippingRate}} (Tells you how much time till you will complete a contract without counting growth)");
                        break;
                    default:
                        await message.Channel.SendMessageAsync($"Unknown Command {command}. Type !help for available commands.");
                        break;
                }
            }
        }
    }
}
