using Discord.WebSocket;
using DiscordCoopCodes.Database;
using DiscordCoopCodes.Database.Entities;
using DiscordCoopCodes.EggIncAPI;
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
    public class CheckForCoops {
        private Timer _timerShort;
        private Timer _timerLong;
        private ApplicationDbContext _db;
        private DiscordSocketClient _client;

        public CheckForCoops(IConfigurationRoot Configuration, DiscordSocketClient client) {
            _db = new ApplicationDbContext(Configuration["ConnectionStrings:DefaultConnection"]);
            _client = client;
            _timerShort = new Timer(async (state) => await DoWorkAsync(state, true), null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            _timerLong = new Timer(async (state) => await DoWorkAsync(state, false), null, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(5));
        }

        public async Task DoWorkAsync(object state, bool isShort) {
            Console.WriteLine($"Checking for new coops {(isShort ? "short" : "long")}");
            var contracts = await _db.Contracts.Where(x => x.GoodUntil > DateTimeOffset.Now).ToListAsync();

            List<Coop> coops;
            if (isShort) {
                coops = await _db.Coops.Where(x => x.ContractID == null && x.Created.AddMinutes(5) > DateTimeOffset.Now).ToListAsync();
            } else {
                coops = await _db.Coops.Where(x => x.ContractID == null && x.Created.AddHours(6) > DateTimeOffset.Now && x.Created.AddMinutes(5) <= DateTimeOffset.Now).ToListAsync();
            }

            foreach (var coop in coops) {
                foreach (var contract in contracts.OrderByDescending(x => x.Created)) {
                    Console.WriteLine($"Checking Coop: {coop.Name} for Contract: {contract.ID} {(isShort ? "short" : "long")}");
                    var status = await ContractsAPI.GetCoopStatus(contract.ID, coop.Name);
                    if (status.Success) {
                        Console.WriteLine("Found Coop for Contract!");
                        coop.ContractID = contract.ID;
                        coop.CoopEnds = DateTimeOffset.Now.AddSeconds(status.TimeLeftSeconds);
                        coop.MaxUsers = contract.MaxUsers;
                        coop.CurrentUsers = status.Participants.Count;

                        _db.CoopStatuses.Add(new CoopStatus {
                            Base64Data = JsonConvert.SerializeObject(status),
                            Created = DateTimeOffset.Now,
                            CoopId = coop.Id
                        });

                        foreach (var p in status.Participants) {
                            var user = await _db.DiscordUsers.FirstOrDefaultAsync(x => x.EggIncIds.ToLower().Contains(p.Name.ToLower()));
                            if (user != null) {
                                _db.UserCoopXrefs.Add(new UserCoopXref { CoopId = coop.Id, DiscordUserId = user.Id, CreatedOn = DateTimeOffset.Now });
                            }
                        }

                        break;
                    } else {
                        Console.WriteLine("Notfound");
                    }

                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
