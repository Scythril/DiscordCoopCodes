using Discord.WebSocket;
using DiscordCoopCodes.Database;
using DiscordCoopCodes.Database.Entities;
using DiscordCoopCodes.EggIncAPI;
using DiscordCoopCodes.Proto;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordCoopCodes.Commands {
    public class Contracts {
        public static async Task MyStatus(SocketMessage message, ApplicationDbContext db) {
            try {
                var user = await db.DiscordUsers.FirstOrDefaultAsync(x => x.DiscordId == message.Author.Id);
                if(user == null) {
                    await message.Channel.SendMessageAsync($"User not registered, please use the command **!addname [eggincusername]**");
                } else {
                    var coops = await db.Coops.Include(x => x.Contract).Where(x => x.UserCoopsXrefs.Any(y => y.DiscordUserId == user.Id)).ToListAsync();
                    var str = "";
                    foreach(var coop in coops) {
                        var coopStatus = await ContractsAPI.GetCoopStatus(coop.Contract.ID, coop.Name);
                        if (coopStatus.Success) {
                            str += await GetStatusString(coopStatus, db);
                        } else {
                            str += "\nERROR: " + coopStatus.Error;
                        }
                    }
                    await message.Channel.SendMessageAsync(str);
                }
            } catch (Exception e) {
                await message.Channel.SendMessageAsync($"ERROR: Bot error - {e.Message}");
            }
        }

        public static async Task AddEggName(SocketMessage message, string[] args, ApplicationDbContext db) {
            try {
                var user = await db.DiscordUsers.FirstOrDefaultAsync(x => x.DiscordId == message.Author.Id);
                if (user == null) {
                    user = new DiscordUser {
                        DiscordId = message.Author.Id,
                        DiscordUsername = message.Author.Username,
                        EggIncIds = args[0],
                        CreateOn = DateTimeOffset.Now
                    };
                    db.DiscordUsers.Add(user);
                } else {
                    user.EggIncIds += "," + args[0];
                }

                var currentCoops = await db.Coops.Where(x => x.CoopEnds > DateTimeOffset.Now).ToListAsync();
                foreach(var coop in currentCoops) {
                    var latestUpdate = await db.CoopStatuses.Where(x => x.CoopId == coop.Id).OrderByDescending(x => x.Created).FirstOrDefaultAsync();
                    if(latestUpdate != null) {
                        var status = JsonConvert.DeserializeObject<CoopStatusProto>(latestUpdate.Base64Data);
                        if(status.Participants.Any(x => user.EggIncIds.ToLower().Contains(x.Name.ToLower()))) {
                            await message.Channel.SendMessageAsync($"Found you in coop: {coop.Name}");

                            db.UserCoopXrefs.Add(new UserCoopXref { CoopId = latestUpdate.CoopId, DiscordUserId = user.Id, CreatedOn = DateTimeOffset.Now });
                        }
                    }
                }

                await db.SaveChangesAsync();

                await message.Channel.SendMessageAsync($"Egg Inc Name: {user.EggIncIds}");
            } catch(Exception e) {
                await message.Channel.SendMessageAsync($"ERROR: Bot error - {e.Message}");
            }
        }

        public static async Task CheckForCoops(SocketMessage message, ApplicationDbContext db) {
            var contractResponse = await ContractsAPI.GetContracts();
            if(contractResponse.Success) {
                foreach(var contract in contractResponse.Contracts) {
                    var coops = await db.Coops.Where(x => x.ContractID == null && x.Created > DateTimeOffset.Now.AddDays(-7)).ToListAsync();
                    foreach(var coop in coops) {
                        var status = await ContractsAPI.GetCoopStatus(contract.ID, coop.Name);
                        if(status.Success) {
                            coop.ContractID = contract.ID;
                            coop.CoopEnds = DateTimeOffset.Now.AddSeconds(status.TimeLeftSeconds);

                            db.CoopStatuses.Add(new CoopStatus {
                                  Base64Data = JsonConvert.SerializeObject(status), Created = DateTimeOffset.Now, CoopId = coop.Id
                            });

                            foreach(var p in status.Participants) {
                                var user = await db.DiscordUsers.FirstOrDefaultAsync(x => x.EggIncIds.ToLower().Contains(p.Name.ToLower()));
                                if(user != null) {
                                    db.UserCoopXrefs.Add(new UserCoopXref { CoopId = coop.Id, DiscordUserId = user.Id, CreatedOn = DateTimeOffset.Now });
                                }
                            }
                        }
                    }
                }
                await db.SaveChangesAsync();
            } else {
                await message.Channel.SendMessageAsync($"ERROR: {contractResponse.Error}");
            }
        }

        //public static async Task GetCurrent(SocketMessage message, ApplicationDbContext db) {
        //    var contractResponse = await ContractsAPI.GetContracts();
        //    if (contractResponse.Success) {
        //        var existingContracts = await db.Contracts.ToListAsync();

        //        foreach (var contract in contractResponse.Contracts) {
        //            if (!existingContracts.Any(x => x.ID == contract.ID)) {
        //                db.Contracts.Add(new Contract {
        //                    ID = contract.ID,
        //                    Created = DateTime.Now,
        //                    Description = contract.Description,
        //                    Name = contract.Name,
        //                    Rewards = JsonConvert.SerializeObject(contract.Rewards),
        //                    P2 = contract.P2,
        //                    P4 = contract.P4,
        //                    P6 = contract.P6,
        //                    P7 = contract.P7,
        //                    P11 = contract.P11,
        //                    GoodUntil = contract.GoodUntil
        //                });
        //                await db.SaveChangesAsync();
        //            }
        //        }

        //        await message.Channel.SendMessageAsync($"Current Contracts:\n  *{String.Join("\n  *", contractResponse.Contracts.Select(x => $"{x.Name} ({x.ID})"))}");

        //    } else {
        //        await message.Channel.SendMessageAsync($"ERROR: {contractResponse.Error}");
        //    }

        //}

        public static async Task GetStatus(SocketMessage message, string[] args, ApplicationDbContext db) {
            if (args.Length < 0 || string.IsNullOrWhiteSpace(args[0])) {
                await message.Channel.SendMessageAsync($"Missing argument Contract ID");
            }

            if (args.Length < 1 || string.IsNullOrWhiteSpace(args[1])) {
                await message.Channel.SendMessageAsync($"Missing argument Coop Name");
            }


            var coop = await ContractsAPI.GetCoopStatus(args[0], args[1]);

            if (coop.Success) {

                await message.Channel.SendMessageAsync(await GetStatusString(coop, db));
            } else {
                await message.Channel.SendMessageAsync($"ERROR: {coop.Error}");
            }
        }

        private static async Task<string> GetStatusString(CoopStatusProto coop, ApplicationDbContext db) {
            var namePad = coop.Participants.Max(x => x.Name.Length);
            var totalPad = Math.Max(coop.Participants.Max(x => x.TotalString.Length), "Total".Length);
            var ratePad = Math.Max(coop.Participants.Max(x => x.RateString.Length), "Rate".Length);

            var projected = coop.Total + coop.Participants.Sum(x => x.Rate) * coop.TimeLeftSeconds;

            var participants = coop.Participants.Select(x => {
                var p = x.Total + x.Rate * coop.TimeLeftSeconds;
                return new {
                    x.Name,
                    x.TotalString,
                    x.RateString,
                    Projected = p,
                    Share = p / projected,
                    x.Sleeping,
                    x.P5
                };
            });


            var contract = await db.Contracts.FirstOrDefaultAsync(x => x.ID == coop.ContractName);

            var pPad = Math.Max(participants.Max(x => x.Projected.ToEggString().Length), "Project".Length);
            var participantstring = string.Join("", participants.Select(x => $"\n{x.Name.PadRight(namePad, ' ')}  {x.TotalString.PadLeft(totalPad)} {x.RateString.PadLeft(ratePad)} {x.Projected.ToEggString().PadLeft(pPad)} {x.Share:P0} {(x.Sleeping ? "Sleeping?" : "")}"));
            var headerString = $"\n{"".PadRight(namePad)}  {"Total".PadRight(totalPad)} {"Rate".PadRight(ratePad)} {"Project".PadRight(pPad)} Share";

            var target = contract.RewardsDetail.Last().Target;

            var str = "";

            if (target < projected) {
                str += "**This coop is projected to succeed without growth as long as there are no sleepers!**\n";
            }

            str += $"**{contract.Name}** for **{coop.CoopName}** Total: **{coop.TotalString}** Projected Total: **{projected.ToEggString()} of {contract.RewardsDetail.Last().Target.ToEggString()}** (with no growth) ```{headerString}{participantstring}```";
            return str;
        }
    }
}
//5/31/2019
//		base64Decoded	"\n?\u0002\n\bstarlink\u0010\n\u001a(\b\u0001\u0011\0??7y?QC\u0018\u0003\"\asubtype)\0\0\0\0??.A1\0\0\0\0??.A\u001a(\b\u0001\u0011\0??7y?aC\u0018\u0004\"\asubtype)\0\0\0\0\0\0??1\0\0@??0?B \u0001(\u00031?i??MB?A9\0\0\0\0\0?'AJ\u0006CleanXR?\u0001To make room for more satellites, we need to clean up space! Over 500,000 pieces of space trash need to be pulled back to earth. So we're building a Graviton Canon!X\0\n?\u0002\n\bnew-moon\u0010\u0004\u001a.\b\u0001\u0011\0\04&?k\fC\u0018\u0005\"\rsilo_capacity)\0\0\0\0\0\0??1\0\0\0\0\0@?@\u001a!\b\u0001\u0011\0???5?JC\u0018\u0006\"\0)\0\0\0\0\0??@1\0\0\0\0??.A\u001a!\b\u0001\u0011\0???W4fC\u0018\u0004\"\0)\0\0\0\0\0\0??1\0\0@??0?B \u0001(\b1??\u0002????A9\0\0\0\0\0u\"AJ\u0010Back to the MoonRYWith renewed plans to colonize the moon, rocket fuel is in high demand. Let us boldly go!X\0\u0011?|??s<?A\u0018\u0006"	string

