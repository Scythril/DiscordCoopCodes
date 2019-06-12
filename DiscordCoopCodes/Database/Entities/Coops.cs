using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordCoopCodes.Database.Entities {
    public class Coop {
        public Guid Id { get; set; }
        public string ContractID { get; set; }
        public string Name { get; set; }

        public int? CurrentUsers { get; set; }
        public int? MaxUsers { get; set; }

        public DateTimeOffset? CoopEnds { get; set; }

        public DateTimeOffset Created { get; set; }

        public Contract Contract { get; set; }
        public List<CoopStatus> CoopStatuses { get; set; }
        public List<UserCoopXref> UserCoopsXrefs { get; set; }
    }
}
