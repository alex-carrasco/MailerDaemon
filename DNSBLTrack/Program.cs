using DNSBLTrack.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DNSBLTrack
{
    class Program
    {
        public static DateTime StartTime = DateTime.Now;
        public static DateTime EndTime;
        public static TimeSpan BatchDuration;
        public static string currentDirectory = Directory.GetCurrentDirectory();
        public static List<BlacklistCollection> blCollection = new List<BlacklistCollection>();
        public static BlacklistsToCheck blacklistsToCheck = null;

        static void Main(string[] args)
        {
            try
            {
                using (StreamReader r = new StreamReader(Path.Combine(currentDirectory, "Blacklists.json")))
                {
                    var json = r.ReadToEnd();

                    blacklistsToCheck = JsonConvert.DeserializeObject<BlacklistsToCheck>(json);
                }

                using (StreamReader r = new StreamReader(Path.Combine(currentDirectory, "IPs.json")))
                {
                    var json = r.ReadToEnd();

                    IPsToCheck items = JsonConvert.DeserializeObject<IPsToCheck>(json);

                    foreach (var provider in items.providerCheck)
                    {
                        Console.WriteLine("Querying ESP: " + provider.ESP);

                        foreach (var i in provider.ipList)
                        {
                            Console.WriteLine("Querying IP: " + i.IP);

                            VerifyIP IP = new VerifyIP(i.IP, blacklistsToCheck);

                            if (IP.IPAddr.Valid)
                                if (IP.BlackList.IsListed)
                                {
                                    BlacklistCollection bl = new BlacklistCollection
                                    {
                                        Provider = provider.ESP,
                                        IPAddress = i.IP,
                                        Blacklist = IP.BlackList.Provider,
                                        Hostname = IP.BlackList.VerifiedOnServer
                                    };

                                    blCollection.Add(bl);

                                    Console.WriteLine("IP {0} listed on {1} - {2}.", i.IP, IP.BlackList.Provider, IP.BlackList.VerifiedOnServer);
                                }
                        }
                        Console.WriteLine("------------------------------------------");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
            finally
            {
                EndTime = DateTime.Now;
                BatchDuration = (EndTime - StartTime);

                Console.WriteLine("App runtime: " + BatchDuration);
            }
        }
    }
}
