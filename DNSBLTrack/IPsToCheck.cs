using System.Collections.Generic;

namespace DNSBLTrack
{
    public class IPsToCheck
    {
        public List<ProviderCheck> providerCheck { get; set; }

        public class ProviderCheck
        {
            public string ESP { get; set; }
            public List<IPList> ipList { get; set; }
        }

        public class IPList
        {
            public string IP { get; set; }
            public string Hostname { get; set; }
        }
    }
}
