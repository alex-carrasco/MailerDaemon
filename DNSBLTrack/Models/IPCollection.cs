using System.Collections.Generic;

namespace DNSBLTrack.Models
{
    public class IPCollection
    {
        public List<ProviderCheck> providerCheck { get; set; }

        public class ProviderCheck
        {
            public string Provider { get; set; }
            public List<IPList> ipList { get; set; }
        }

        public class IPList
        {
            public string IP { get; set; }
            public string Hostname { get; set; }
        }
    }
}
