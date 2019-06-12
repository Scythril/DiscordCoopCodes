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
    public class CoopStatusUpdater {
        private Timer _timer;
        private ApplicationDbContext _db;
        private DiscordSocketClient _client;

        public CoopStatusUpdater(IConfigurationRoot Configuration, DiscordSocketClient client) {
            _db = new ApplicationDbContext(Configuration["ConnectionStrings:DefaultConnection"]);
            _client = client;
            _timer = new Timer(async (state) => await DoWorkAsync(state, true), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        public async Task DoWorkAsync(object state, bool isShort) {
            var coops = await _db.Coops.Include(x => x.Contract).Where(x => x.CoopEnds.HasValue && x.CoopEnds.Value.AddDays(1) > DateTimeOffset.Now).ToListAsync();
            var users = await _db.DiscordUsers.Include(x => x.UserCoopXrefs).ToListAsync();


            foreach (var coop in coops) {
                Console.WriteLine($"Checking Coop: {coop.Name} for status update");
                var status = await ContractsAPI.GetCoopStatus(coop.ContractID, coop.Name);
                if (status.Success) {
                    _db.CoopStatuses.Add(new CoopStatus {
                        Base64Data = JsonConvert.SerializeObject(status),
                        Created = DateTimeOffset.Now,
                        CoopId = coop.Id
                    });

                    if (coop.CurrentUsers != status.Participants.Count) {
                        coop.CurrentUsers = status.Participants.Count;
                        coop.MaxUsers = coop.Contract.MaxUsers;
                    }

                    foreach (var p in status.Participants) {
                        var user = users.FirstOrDefault(x => !x.UserCoopXrefs.Any(y => y.CoopId == coop.Id) && x.EggIncIds.ToLower().Contains(p.Name.ToLower()));
                        if (user != null) {
                            _db.UserCoopXrefs.Add(new UserCoopXref { CoopId = coop.Id, DiscordUserId = user.Id, CreatedOn = DateTimeOffset.Now });
                            Console.WriteLine("Adding user xref");
                        }
                    }
                } else {
                    Console.WriteLine("Notfound coop for status update");
                }

            }

            await _db.SaveChangesAsync();
        }
    }
}
