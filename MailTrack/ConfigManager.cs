using System;
using System.Configuration;

namespace MailTrack
{
    public class ConfigManager
    {
        private static readonly ConfigManager instance = new ConfigManager();
        public static ConfigManager Instance
        {
            get
            {
                return instance;
            }
        }

        static ConfigManager()
        {
        }

        private ConfigManager()
        {
            LoadData();
        }

        private string m_SMTPServer = null;
        public string SMTPServer
        {
            get
            {
                return m_SMTPServer;
            }
        }

        private string m_EmailFrom = null;
        public string EmailFrom
        {
            get
            {
                return m_EmailFrom;
            }
        }

        private string m_EmailTo = null;
        public string EmailTo
        {
            get
            {
                return m_EmailTo;
            }
        }

        private int m_CacheTTLMinutes = 0;
        public int CacheTTLMinutes
        {
            get
            {
                return m_CacheTTLMinutes;
            }
        }

        private bool m_ForceNoCache = false;
        public bool ForceNoCache
        {
            get
            {
                return m_ForceNoCache;
            }
        }

        private string m_IPLogRestriction = null;
        public string IPLogRestriction
        {
            get
            {
                return m_IPLogRestriction;
            }
        }

        private string m_DBConnectionString = null;
        public string DBConnectionString
        {
            get
            {
                return m_DBConnectionString;
            }
        }

        private void LoadData()
        {
            lock (this)
            {
                m_SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
                m_EmailFrom = ConfigurationManager.AppSettings["EmailFrom"];
                m_EmailTo = ConfigurationManager.AppSettings["EmailTo"];
                m_CacheTTLMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["CacheTTLMinutes"]);
                m_ForceNoCache = Convert.ToBoolean(ConfigurationManager.AppSettings["ForceNoCache"]);
                m_IPLogRestriction = ConfigurationManager.AppSettings["IPLogRestriction"];
                m_DBConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            }
        }
    }
}