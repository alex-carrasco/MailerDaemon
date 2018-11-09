using System.Collections.Generic;

namespace DNSBLTrack
{
    public class BlacklistsToCheck
    {
        public List<BlacklistCheck> blacklistCheck { get; set; }

        public class BlacklistCheck
        {
            public string Provider { get; set; }
            public List<DNSBLList> dnsblList { get; set; }
        }

        public class DNSBLList
        {
            public string HostName { get; set; }
        }
    }
}
