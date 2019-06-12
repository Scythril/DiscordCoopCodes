using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordCoopCodes.Proto {
    [ProtoContract]
    public class ContractsProto {
        [ProtoMember(1)]
        public List<ContractProto> Contracts { get; set; }
        [ProtoMember(2)]
        public double U1 { get; set; }
        [ProtoMember(3)]
        public int U2 { get; set; }

        public bool Success { get; set; }
        public string Error { get; set; }
    }

    [ProtoContract]
    public class ContractProto {
        [ProtoMember(1)]
        public string ID { get; set; }
        [ProtoMember(2)]
        public int P2 { get; set; }
        [ProtoMember(3)]
        public List<RewardProto> Rewards { get; set; }
        [ProtoMember(4)]
        public int P4 { get; set; }
        [ProtoMember(5)]
        public int Size { get; set; }
        [ProtoMember(6)]
        public double P6 { get; set; }
        [ProtoMember(7)]
        public double P7 { get; set; }
        [ProtoMember(9)]
        public string Name { get; set; }
        [ProtoMember(10)]
        public string Description { get; set; }
        [ProtoMember(11)]
        public int P11 { get; set; }

        public TimeSpan ContractTime { get { return TimeSpan.FromSeconds(P7); } }
        public DateTimeOffset GoodUntil {  get { return DateTimeOffset.FromUnixTimeSeconds((long)P6); } }
        public  bool AllowCoop {  get { return P4 == 1; } }
    }

    [ProtoContract]
    public class RewardProto {
        [ProtoMember(1)]
        public int P1 { get; set; }
        [ProtoMember(2)]
        public double Target { get; set; }
        public string TargetStr { get { return ArgumentsHelper.NumberToString(Target); } }
        [ProtoMember(3)]
        public int Icon { get; set; }
        [ProtoMember(4)]
        public string Type { get; set; }
        [ProtoMember(5)]
        public double Amount { get; set; }
        [ProtoMember(6)]
        public double P6 { get; set; }

        public string Name {
            get {
                if (Type.Length > 0 && Type != "subtype") {
                    return String.Join(" ", Type.Split("_").Select(x => x.First().ToString().ToUpper() + x.Substring(1)));
                }
                switch (Icon) {
                    case 2:
                        return "Golden Eggs";
                    case 3:
                        return "Soul Eggs";
                    case 4:
                        return "Prophecy Eggs";
                }
                return "Unknown";
            }
        }
    }
}
