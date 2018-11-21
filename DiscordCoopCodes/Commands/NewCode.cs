using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCoopCodes.Commands {
    public class NewCode {
        public static async Task ExecuteAsync(SocketMessage message) {
            var words = new Words();
            var code = words.GetRandomWord() + words.GetRandomWord() + words.GetRandomNumber();
            await message.Channel.SendMessageAsync(code);
        }
    }
}
