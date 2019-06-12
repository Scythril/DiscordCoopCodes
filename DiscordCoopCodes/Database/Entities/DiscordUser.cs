using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordCoopCodes.Database.Entities
{
    public class DiscordUser
    {
        public Guid Id { get; set; }
        public ulong DiscordId { get; set; }
        public string DiscordUsername { get; set; }
        public string EggIncIds { get; set; }

        public DateTimeOffset CreateOn { get; set; }

        public List<UserCoopXref> UserCoopXrefs { get; set; }
    }
}
