using DiscordCoopCodes.Proto;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCoopCodes.EggIncAPI {
    public class ContractsAPI {
        public static async Task<ContractsProto> GetContracts() {
            try {
                using (var client = new HttpClient()) {
                    client.BaseAddress = new Uri("http://www.auxbrain.com/");
                    var response = await client.PostAsync("ei/get_contracts", null);

                    if (response.IsSuccessStatusCode) {
                        var r = await response.Content.ReadAsStringAsync();
                        var responseString = System.Convert.FromBase64String(r);

                        var ms = new MemoryStream();
                        ms.Write(responseString);
                        ms.Position = 0;

                        var c = Serializer.Deserialize<Proto.ContractsProto>(ms);
                        c.Success = true;

                        return c;
                    } else {
                        return new ContractsProto { Success = false, Error = "Error response from API" };
                    }
                }
            } catch (Exception e) {
                return new ContractsProto { Success = false, Error = "Bot Exception: " + e.Message };
            }
        }

        public static async Task<CoopStatusProto> GetCoopStatus(string ContractName, string CoopName) {
            try {
                using (var client = new HttpClient()) {
                    client.BaseAddress = new Uri("http://www.auxbrain.com/");

                    var ms1 = new MemoryStream();
                    Serializer.Serialize<Proto.CoopRequestProto>(ms1, new Proto.CoopRequestProto { ContractName = ContractName, CoopName = CoopName.ToLower() });
                    ms1.Position = 0;
                    var sr = new StreamReader(ms1);
                    var base64 = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(sr.ReadToEnd()));
                    var bac = new ByteArrayContent(ASCIIEncoding.ASCII.GetBytes("data=" + base64));
                    client.DefaultRequestHeaders.Add("User-Agent", "Dalvik/2.1.0 (Linux; U; Android 9; SM-G960U1 Build/PPR1.180610.011)");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                    bac.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    var response = await client.PostAsync("ei/coop_status", bac);

                    if (response.IsSuccessStatusCode) {
                        var r = await response.Content.ReadAsStringAsync();
                        var responseString = System.Convert.FromBase64String(await response.Content.ReadAsStringAsync());

                        var ms = new MemoryStream();
                        ms.Write(responseString);
                        ms.Position = 0;

                        var coop = Serializer.Deserialize<Proto.CoopStatusProto>(ms);
                        coop.Success = true;

                        return coop;
                    } else {
                        return new CoopStatusProto { Success = false, Error = "Error response from API" };
                    }
                }

            } catch (Exception e) {
                return new CoopStatusProto { Success = false, Error = "Bot Exception: " + e.Message };
            }
        }

        public static async Task<FirstContactResponseProto> FirstContact(string UserId) {
            try {
                using (var client = new HttpClient()) {
                    client.BaseAddress = new Uri("http://www.auxbrain.com/");

                    var ms1 = new MemoryStream();
                    Serializer.Serialize<FirstContactRequestProto>(ms1, new FirstContactRequestProto { UserId = UserId, P2 = 0, P3 = 2 });
                    ms1.Position = 0;
                    var sr = new StreamReader(ms1);
                    var base64 = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(sr.ReadToEnd()));
                    var bac = new ByteArrayContent(ASCIIEncoding.ASCII.GetBytes("data=" + base64));
                    client.DefaultRequestHeaders.Add("User-Agent", "Dalvik/2.1.0 (Linux; U; Android 9; SM-G960U1 Build/PPR1.180610.011)");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                    bac.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    var response = await client.PostAsync("ei/first_contact", bac);

                    if (response.IsSuccessStatusCode) {
                        var r = await response.Content.ReadAsStringAsync();
                        var responseString = System.Convert.FromBase64String(await response.Content.ReadAsStringAsync());


                        


                            var ms = new MemoryStream();
                        ms.Write(responseString);
                        ms.Position = 0;


                        var reader = Serializer

                        return null;

                        //var coop = Serializer.Deserialize<FirstContactResponseProto>(ms);
                        //coop.Success = true;

                        //return coop;
                    } else {
                        return new FirstContactResponseProto { Success = false, Error = "Error response from API" };
                    }
                }

            } catch (Exception e) {
                return new FirstContactResponseProto { Success = false, Error = "Bot Exception: " + e.Message };
            }
        }
    }
}
