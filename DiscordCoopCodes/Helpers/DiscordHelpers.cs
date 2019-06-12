using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCoopCodes.Helpers {
    public static class DiscordHelpers {
        public static async Task SentToContractsChannelAsync(this DiscordSocketClient client, string msg) {
            foreach (var channel in client.GroupChannels.Where(x => x.Name == "current-contract-discussion")) {
                await channel.SendMessageAsync(msg);
            }
        }
    }
}
