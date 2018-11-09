using System;
using System.Net;
using System.Net.Sockets;

namespace DNSBLTrack
{
    public class VerifyIP
    {
        public class ExIPAddress
        {
            private string[] _adresse;
            private bool _valid;

            public bool Valid
            {
                get { return _valid; }
            }

            public string AsString
            {
                get
                {
                    if (_valid)
                    {
                        string tmpstr = "";
                        for (int ai = 0; ai < _adresse.Length; ai++)
                        {
                            tmpstr += _adresse[ai];
                            if (ai < _adresse.Length - 1)
                                tmpstr += ".";
                        }
                        return tmpstr;
                    }
                    else
                        return "";
                }
                set
                {
                    _adresse = value.Split(new Char[] { '.' });
                    if (_adresse.Length == 4)
                    {
                        try
                        {
                            _valid = true;
                            byte tmpx = 0;
                            foreach (string addsec in _adresse)
                                if (!byte.TryParse(addsec, out tmpx))
                                {
                                    _valid = false;
                                    break;
                                }
                        }
                        catch { _valid = false; }
                    }
                    else
                        _valid = false;
                }
            }

            public string AsRevString
            {
                get
                {
                    if (_valid)
                    {
                        string tmpstr = "";
                        for (int ai = _adresse.Length - 1; ai >= 0; ai--)
                        {
                            tmpstr += _adresse[ai];
                            if (ai > 0)
                                tmpstr += ".";
                        }
                        return tmpstr;
                    }
                    else
                        return "";
                }
            }

            public string[] AsStringArray
            {
                get { return _adresse; }
                set
                {
                    if (value.Length == 4)
                    {
                        try
                        {
                            _valid = true;
                            byte tmpx = 0;
                            foreach (string addsec in value)
                                if (!byte.TryParse(addsec, out tmpx))
                                {
                                    _valid = false;
                                    break;
                                }
                        }
                        catch { _valid = false; }
                    }
                    else
                        _valid = false;
                }
            }

            public string[] AsRevStringArray
            {
                get
                {
                    string[] tmpstrarr = new string[_adresse.Length];
                    for (int ai = _adresse.Length - 1; ai >= 0; ai--)
                    {
                        tmpstrarr[(_adresse.Length - 1) - ai] = _adresse[ai];
                    }
                    return tmpstrarr;
                }
            }

            public byte[] AsByteArray
            {
                get
                {
                    if (_valid)
                        return StringToByte(_adresse);
                    else
                        return new byte[0];
                }
                set
                {
                    if (value.Length == 4)
                    {
                        _adresse = ByteToString(value);
                        _valid = true;
                    }
                    else
                        _valid = false;
                }
            }

            public byte[] AsRevByteArray
            {
                get
                {
                    byte[] tmpcon = StringToByte(_adresse);
                    byte[] tmpbytearr = new byte[tmpcon.Length];
                    for (int ai = tmpcon.Length - 1; ai >= 0; ai--)
                    {
                        tmpbytearr[(tmpcon.Length - 1) - ai] = tmpcon[ai];
                    }
                    return tmpbytearr;
                }
            }

            public long AsLong
            {
                get
                {
                    if (_valid)
                        return StringToLong(_adresse, true);
                    else
                        return 0;
                }
                set
                {
                    try
                    {
                        _adresse = LongToString(value, false);
                        _valid = true;
                    }
                    catch { _valid = false; }
                }
            }

            public long AsRevLong
            {
                get { return StringToLong(_adresse, false); }
            }

            public ExIPAddress() { }

            public ExIPAddress(string address)
            {
                this.AsString = address;
            }

            public ExIPAddress(string[] address)
            {
                this.AsStringArray = address;
            }

            public ExIPAddress(byte[] address)
            {
                this.AsByteArray = address;
            }

            public ExIPAddress(long address)
            {
                this.AsLong = address;
            }

            private byte[] StringToByte(string[] strArray)
            {
                try
                {
                    byte[] tmp = new byte[strArray.Length];
                    for (int ia = 0; ia < strArray.Length; ia++)
                        tmp[ia] = byte.Parse(strArray[ia]);
                    return tmp;
                }
                catch
                {
                    return new byte[0];
                }
            }

            private string[] ByteToString(byte[] byteArray)
            {
                try
                {
                    string[] tmp = new string[byteArray.Length];

                    for (int ia = 0; ia < byteArray.Length; ia++)
                        tmp[ia] = byteArray[ia].ToString();
                    return tmp;
                }
                catch
                {
                    return new string[0];
                }
            }

            private long StringToLong(string[] straddr, bool Revese)
            {
                long num = 0;
                if (straddr.Length == 4)
                {
                    try
                    {
                        if (Revese)
                            num = (int.Parse(straddr[0])) +
                                (int.Parse(straddr[1]) * 256) +
                                (int.Parse(straddr[2]) * 65536) +
                                (int.Parse(straddr[3]) * 16777216);
                        else
                            num = (int.Parse(straddr[3])) +
                                (int.Parse(straddr[2]) * 256) +
                                (int.Parse(straddr[1]) * 65536) +
                                (int.Parse(straddr[0]) * 16777216);
                    }
                    catch { num = 0; }
                }
                else
                    num = 0;
                return num;
            }

            private string[] LongToString(long lngval, bool Revese)
            {
                string[] tmpstrarr = new string[4];
                if (lngval > 0)
                {
                    try
                    {
                        int a = (int)(lngval / 16777216) % 256;
                        int b = (int)(lngval / 65536) % 256;
                        int c = (int)(lngval / 256) % 256;
                        int d = (int)(lngval) % 256;
                        if (Revese)
                        {
                            tmpstrarr[0] = a.ToString();
                            tmpstrarr[1] = b.ToString();
                            tmpstrarr[2] = c.ToString();
                            tmpstrarr[3] = d.ToString();
                        }
                        else
                        {
                            tmpstrarr[3] = a.ToString();
                            tmpstrarr[2] = b.ToString();
                            tmpstrarr[1] = c.ToString();
                            tmpstrarr[0] = d.ToString();
                        }
                    }
                    catch { }
                    return tmpstrarr;
                }
                else
                    return tmpstrarr;
            }
        }

        public class BlackListed
        {
            private bool _IsListed;
            private string _verifiedonserver;
            private string _provider;

            public string VerifiedOnServer
            {
                get { return _verifiedonserver; }
            }

            public bool IsListed
            {
                get { return _IsListed; }
            }

            public string Provider
            {
                get { return _provider; }
            }

            public BlackListed(bool listed, string server)
            {
                this._IsListed = listed;
                this._verifiedonserver = server;
            }

            public BlackListed(bool listed, string server, string provider)
            {
                this._IsListed = listed;
                this._verifiedonserver = server;
                this._provider = provider;
            }
        }

        private ExIPAddress _ip;
        private BlackListed _blacklisted = new BlackListed(false, "");

        public ExIPAddress IPAddr
        {
            get { return _ip; }
            set { _ip = value; }
        }

        public BlackListed BlackList
        {
            get { return _blacklisted; }
        }

        public VerifyIP(byte[] address, string[] blacklistservers)
        {
            _ip = new ExIPAddress(address);
            VerifyOnServers(blacklistservers);
        }

        public VerifyIP(long address, string[] blacklistservers)
        {
            _ip = new ExIPAddress(address);
            VerifyOnServers(blacklistservers);
        }

        public VerifyIP(string address, string[] blacklistservers)
        {
            _ip = new ExIPAddress(address);
            VerifyOnServers(blacklistservers);
        }

        public VerifyIP(string address, BlacklistsToCheck blacklistServers)
        {
            _ip = new ExIPAddress(address);
            VerifyOnServers(blacklistServers);
        }

        public VerifyIP(ExIPAddress address, string[] blacklistservers)
        {
            _ip = address;
            VerifyOnServers(blacklistservers);
        }

        private void VerifyOnServers(string[] _blacklistservers)
        {
            _blacklisted = null;
            if (_blacklistservers != null && _blacklistservers.Length > 0)
            {
                foreach (string BLSrv in _blacklistservers)
                {
                    if (VerifyOnServer(BLSrv))
                    {
                        _blacklisted = new BlackListed(true, BLSrv);
                        break;
                    }
                }
                if (_blacklisted == null)
                    _blacklisted = new BlackListed(false, "");
            }
        }

        private void VerifyOnServers(BlacklistsToCheck _blacklistServers)
        {
            _blacklisted = null;
            if (_blacklistServers != null && _blacklistServers.blacklistCheck.Count > 0)
            {
                foreach (var item in _blacklistServers.blacklistCheck)
                {
                    foreach (var i in item.dnsblList)
                    {
                        if (VerifyOnServer(i.HostName))
                        {
                            _blacklisted = new BlackListed(true, i.HostName, item.Provider);
                            break;
                        }
                    }
                    if (_blacklisted == null)
                        _blacklisted = new BlackListed(false, "");
                }
            }
        }

        private bool VerifyOnServer(string BLServer)
        {
            if (_ip.Valid)  //If IP address is valid continue..
            {
                try
                {
                    IPHostEntry ipEntry = Dns.GetHostEntry(_ip.AsRevString + "." + BLServer);  //Look up the IP address on the BLServer
                    ipEntry = null; //Clear the object
                    return true; //IP address was found on the BLServer,
                                 //it's then listed in the black list
                }
                catch (SocketException dnserr)
                {
                    if (dnserr.ErrorCode == 11001) // IP address not listed
                        return false;
                    else // Another error happened, put other error handling here
                        return false;
                }
            }
            else
                return false;   //IP address is not valid
        }
    }
}
