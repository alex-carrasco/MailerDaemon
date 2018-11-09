namespace DNSBLTrack.Models
{
    public class BlacklistCollection
    {
        public string Provider { get; set; }
        public string IPAddress { get; set; }
        public string Blacklist { get; set; }
        public string Hostname { get; set; }

        public BlacklistCollection(string Provider, string IPAddress, string Blacklist, string Hostname)
        {
            this.Provider = Provider;
            this.IPAddress = IPAddress;
            this.Blacklist = Blacklist;
            this.Hostname = Hostname;
        }

        public BlacklistCollection() { }
    }
}
