using Discord.WebSocket;
using DiscordCoopCodes.Database;
using DiscordCoopCodes.Database.Entities;
using DiscordCoopCodes.EggIncAPI;
using DiscordCoopCodes.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordCoopCodes.Automated {
    public class CheckForNewContracts {
        private Timer _timer;
        private ApplicationDbContext _db;
        private DiscordSocketClient _client;

        public CheckForNewContracts(IConfigurationRoot Configuration, DiscordSocketClient client) {
            _db = new ApplicationDbContext(Configuration["ConnectionStrings:DefaultConnection"]);
            _client = client;
            _timer = new Timer(async (state) => await DoWorkAsync(state), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        public async Task DoWorkAsync(object state) {
            var contractResponse = await ContractsAPI.GetContracts();
            if (contractResponse.Success) {
                Console.WriteLine("Checking for new contracts");
                var existingContracts = await _db.Contracts.ToListAsync();

                foreach (var contract in contractResponse.Contracts) {
                    if (!existingContracts.Any(x => x.ID == contract.ID)) {
                        _db.Contracts.Add(new Contract {
                            ID = contract.ID,
                            Created = DateTime.Now,
                            Description = contract.Description,
                            Name = contract.Name,
                            Rewards = JsonConvert.SerializeObject(contract.Rewards),
                            P2 = contract.P2,
                            P4 = contract.P4,
                            P6 = contract.P6,
                            P7 = contract.P7,
                            P11 = contract.P11,
                            GoodUntil = contract.GoodUntil
                        });

                        await _client.SentToContractsChannelAsync($"New Contract!\n");
                        foreach (var channel in _client.GroupChannels.Where(x => x.Name == "current-contract-discussion")) {
                            var msg = "";

                            msg += "A new oontract has been listed.\n";
                            msg += $"{contract.Name} ({contract.ID})\n";
                            msg += $"{contract.Description}\n";
                            msg += $"**Rewards**\n";
                            foreach(var reward in contract.Rewards) {
                                msg += $" - {reward.Amount} **{reward.Name}** Target: **{reward.Target.ToEggString()}**\n";
                            }


                            await channel.SendMessageAsync(msg);
                        }
                    }
                }
            } else {
                Console.WriteLine("ERROR: Invalid Contract Response");
            }

            await _db.SaveChangesAsync();

        }
    }
}
