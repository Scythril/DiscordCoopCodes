using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordCoopCodes.Database.Entities
{
    public class UserCoopXref
    {
        public Guid DiscordUserId { get; set; }
        public Guid CoopId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

        public Coop Coop { get; set; }
        public DiscordUser DiscordUser { get; set; }
    }
}
