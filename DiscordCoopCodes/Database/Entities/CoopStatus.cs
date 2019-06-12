using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordCoopCodes.Database.Entities
{
    public class CoopStatus
    {
        public Guid Id { get; set; }

        public Guid CoopId { get; set; }

        public Coop Coop { get; set; }

        public DateTimeOffset Created { get; set; }


        public string Base64Data { get; set; }
    }
}
