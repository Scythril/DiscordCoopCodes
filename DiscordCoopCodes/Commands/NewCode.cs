using Discord.WebSocket;
using DiscordCoopCodes.Database;
using DiscordCoopCodes.Database.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCoopCodes.Commands {
    public class NewCode {
        public static async Task ExecuteAsync(SocketMessage message, ApplicationDbContext db) {
            var words = new Words();
            var code = words.GetRandomWord() + words.GetRandomWord() + words.GetRandomNumber();


            var coop = new Coop { Name = code, Created = DateTimeOffset.Now };
            db.Coops.Add(coop);
            await db.SaveChangesAsync();
                
            await message.Channel.SendMessageAsync(code);
        }
    }
}
