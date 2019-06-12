using DiscordCoopCodes.Proto;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;

namespace DiscordCoopCodes.Database.Entities {
    public class Contract {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Rewards { get; set; }

        public int P2 { get; set; }
        public int P4 { get; set; }
        public double P6 { get; set; }
        public double P7 { get; set; }
        public int P11 { get; set; }

        public DateTimeOffset GoodUntil { get; set; }

        [NotMapped]
        public TimeSpan ContractTime { get { return TimeSpan.FromSeconds(P7); } }
        [NotMapped]
        public int MaxUsers { get { return P2; } }
        [NotMapped]
        public List<RewardProto> RewardsDetail {
            get {
                return JsonConvert.DeserializeObject<List<RewardProto>>(Rewards);
            }
        }

        public DateTimeOffset Created { get; set; }

        public List<Coop> Coops { get; set; }
    }
}
