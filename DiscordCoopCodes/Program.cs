using Discord;
using Discord.WebSocket;
using DiscordCoopCodes;
using DiscordCoopCodes.Automated;
using DiscordCoopCodes.Commands;
using DiscordCoopCodes.Database;
using DiscordCoopCodes.EggIncAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordCoopCords {
    class Program {
        private static IConfigurationRoot Configuration;
        private static ApplicationDbContext db;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() {
            Configuration = new ConfigurationBuilder()
                .AddUserSecrets<Secrets>()
                .Build();

            var client = new DiscordSocketClient();

            db = new ApplicationDbContext(Configuration["ConnectionStrings:DefaultConnection"]);



            client.Log += Log;
            client.MessageReceived += MessageReceived;

            await client.LoginAsync(TokenType.Bot, Configuration["Token"]);
            await client.StartAsync();

            //await Contracts.ExecuteAsync(null);

            //await Contracts.GetStatus(null, new string[] { "starlink", "uppityshare62" } );
            //await Contracts.GetStatus(null, new string[] { "starlink", "textureroof15" });

            // Block this task until the program is closed.

#if !DEBUG
            var checkForNewContracts = new CheckForNewContracts(Configuration, client);
            var checkForCoops = new CheckForCoops(Configuration, client);
            var coopStatusUpdater = new CoopStatusUpdater(Configuration, client);
#endif


            var firstcontact = await ContractsAPI.FirstContact("102371659776481580429");

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
                    case "mystatus": await Contracts.MyStatus(message, db); break;
                    case "addname": await Contracts.AddEggName(message, args, db); break;
                    case "checkforcoops": await Contracts.CheckForCoops(message, db); break;
                    case "ping": await Ping.ExecuteAsync(message); break;
                    case "newcode": await NewCode.ExecuteAsync(message, db); break;
                    case "contracttime": await TimeRemaining.ExecuteAsync(message, args); break;
                    //case "currentcontracts": await Contracts.GetCurrent(message, db); break;
                    case "getstatus": await Contracts.GetStatus(message, args, db); break;
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
