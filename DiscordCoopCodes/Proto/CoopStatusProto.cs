using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordCoopCodes.Proto
{
    [ProtoContract]
    public class CoopRequestProto
    {
        [ProtoMember(1)]
        public string ContractName { get; set; }
        [ProtoMember(2)]
        public string CoopName { get; set; }
    }

    [ProtoContract]
    public class CoopStatusProto
    {
        [ProtoMember(1)]
        public string ContractName { get; set; }
        [ProtoMember(2)]
        public double Total { get; set; }
        public string TotalString { get { return ArgumentsHelper.NumberToString(Total); } }
        [ProtoMember(3)]
        public string CoopName { get; set; }
        [ProtoMember(4)]
        public List<byte[]> ParticipantProto { get; set; }
        [ProtoMember(5)]
        public double TimeLeftSeconds { get; set; }
        [ProtoMember(6)]
        public int P6 { get; set; }
        [ProtoMember(7)]
        public double Rate { get; set; }
        public string RatePerHour { get { return ArgumentsHelper.NumberToString(Rate * 60 * 60) + "/HR"; } }


        public List<CoopParticipantProto> Participants { get {
                var o = new List<CoopParticipantProto>();
                if (ParticipantProto == null)
                    return o;
                foreach (var p in ParticipantProto)
                {
                    var base64 = Convert.ToBase64String(p);
                    var ms = new MemoryStream();
                    ms.Write(p);
                    ms.Position = 0;
                    //try
                    {
                        o.Add(Serializer.Deserialize<Proto.CoopParticipantProto>(ms));
                    }
                    //catch { }
                }

                return o;
            }
        }

        public bool Success { get; set; }
        public string Error { get; set; }
    }

    [ProtoContract]
    public class CoopParticipantProto
    {
        [ProtoMember(1)]
        public string P1 { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public double Total { get; set; }
        public string TotalString { get { return ArgumentsHelper.NumberToString(Total); } }
        [ProtoMember(4)]
        public int P4 { get; set; }
        [ProtoMember(5)]
        public int P5 { get; set; }
        [ProtoMember(6)]
        public double Rate { get; set; }
        public string RateString { get { return ArgumentsHelper.NumberToString(Rate) + "/SEC"; } }
        public bool Sleeping { get { return P4 == 0; } }
    }
}
