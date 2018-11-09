using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace MailTrack
{
    public class ot : IHttpAsyncHandler
    {
        private ConfigManager ConfigItems = ConfigManager.Instance;
        protected byte[] m_imgbytes = Convert.FromBase64String("R0lGODlhAQABAIAAANvf7wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==");
        protected HttpContext m_context = null;
        protected SqlCommand m_cmdSQL = null;

        public void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException("This class should only be used asynchronously!");
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            IAsyncResult ar = null;
            int MailID;
            string Hash = null;
            string ClientIP = null;
            bool IsMobile;
            string PlatformName = null;
            string BrowserName = null;
            bool ErrorPresent = false;
            m_context = context;

            try
            {
                if ((m_context.Request.QueryString["m"] == null) || (m_context.Request.QueryString["h"] == string.Empty))
                {
                    m_context.Response.StatusCode = 400;
                    m_context.Response.End();
                    NotificationManager.Send(ConfigItems.SMTPServer, ConfigItems.EmailFrom, ConfigItems.EmailTo, "Error in mail tracking", "Error during BeginProcessRequest(). Missing or invalid MailID/Hash querystring parameter(s)!", true, HttpContext.Current.Request, true, DateTime.UtcNow);
                    return new EmptyAsyncResult(ref cb, ref extraData, true, true);
                }
                else
                {
                    if ((m_context.Request.QueryString["i"] == null) || (m_context.Request.QueryString["i"] == string.Empty))
                        ClientIP = m_context.Request.UserHostAddress;
                    else
                        ClientIP = m_context.Request.QueryString["i"].ToString();

                    MailID = Convert.ToInt32(m_context.Request.QueryString["m"]);
                    Hash = m_context.Request.QueryString["h"];
                    IsMobile = Convert.ToBoolean(m_context.Request.Browser["IsMobile"]);
                    PlatformName = m_context.Request.Browser["PlatformName"];
                    BrowserName = m_context.Request.Browser["BrowserName"];
                }

                // don't record your own IP range or specific IP
                if (ClientIP.StartsWith(ConfigItems.IPLogRestriction) || ClientIP == ConfigItems.IPLogRestriction)
                {
                    m_context.Response.StatusCode = 403;
                    m_context.Response.SuppressContent = true;
                    m_context.Response.End();
                    // DebugEmail.Send(ConfigItems.SMTPServer, ConfigItems.EmailFrom, ConfigItems.EmailTo, "Notification - Open attempt blocked", "There has been an open operation performed within an excluded IP or IP range.", True, HttpContext.Current.Request, True, DateTime.UtcNow())
                    return new EmptyAsyncResult(ref cb, ref extraData, true, true);
                }

                // check if-modified-since header to determine if we should log again or send back a not modified result
                if (UseCached(m_context.Request))
                {
                    m_context.Response.StatusCode = 304;
                    m_context.Response.SuppressContent = true;
                    m_context.Response.End();
                    return new EmptyAsyncResult(ref cb, ref extraData, true, true);
                }
                else
                {
                    // log the event to the database
                    ar = LogData(ConfigItems.DBConnectionString, ref cb, ref extraData, MailID, Hash, ClientIP, IsMobile, PlatformName, BrowserName);
                    // stream our pixel to the client and set cacheability and last modified appropriately
                    TimeSpan CacheTTL = new TimeSpan(0, ConfigItems.CacheTTLMinutes, 0);
                    m_context.Response.ContentType = "image/gif";
                    m_context.Response.AppendHeader("Content-Length", m_imgbytes.Length.ToString());
                    m_context.Response.Cache.SetLastModified(DateTime.Now);
                    if (ConfigItems.ForceNoCache == true)
                        m_context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    else
                    {
                        m_context.Response.Cache.SetCacheability(HttpCacheability.Public);
                        m_context.Response.Cache.SetExpires(DateTime.Now.Add(CacheTTL));
                        m_context.Response.Cache.SetMaxAge(CacheTTL);
                    }
                    m_context.Response.BinaryWrite(m_imgbytes);
                    m_context.Response.End();
                    return ar;
                }
            }
            catch (Exception ex)
            {
                ErrorPresent = true;
                m_context.Response.StatusCode = 500;
                m_context.Response.End();
                NotificationManager.Send(ConfigItems.SMTPServer, ConfigItems.EmailFrom, ConfigItems.EmailTo, "Error in email tracking", "Error during BeginProcessRequest().<br>" + ex.ToString(), true, HttpContext.Current.Request, true, DateTime.UtcNow);
                return new EmptyAsyncResult(ref cb, ref extraData, true, true);
            }

            finally
            {
                if (ErrorPresent)
                {
                    if (m_cmdSQL != null)
                    {
                        if (m_cmdSQL.Connection.State != ConnectionState.Closed)
                            m_cmdSQL.Connection.Close();
                    }
                    m_cmdSQL = null;
                }
            }
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            if (m_cmdSQL != null)
            {
                try
                {
                    m_cmdSQL.EndExecuteNonQuery(result);
                }
                catch (Exception ex)
                {
                    NotificationManager.Send(ConfigItems.SMTPServer, ConfigItems.EmailFrom, ConfigItems.EmailTo, "Error in email tracking", "Error during EndProcessRequest().<br>" + ex.ToString(), true, m_context.Request);
                }
                finally
                {
                    // make sure we close the connection before returning from this method
                    if (m_cmdSQL.Connection.State != ConnectionState.Closed)
                        m_cmdSQL.Connection.Close();
                    m_cmdSQL = null;
                }
            }
        }

        private bool UseCached(HttpRequest req)
        {
            if (ConfigItems.CacheTTLMinutes == 0)
                return false;
            string ifmod = req.Headers["If-Modified-Since"];
            if ((ifmod == null) || (ifmod == string.Empty))
                return false;
            else
            {
                if (DateTime.TryParse(ifmod, out DateTime IfModDate) == true)
                    return IfModDate.AddMinutes(ConfigItems.CacheTTLMinutes) >= DateTime.Now;
                else
                    return false;
            }
        }

        private IAsyncResult LogData(string ConnectionString, ref AsyncCallback cb, ref object extraData, int MailID, string Hash, string IPAddress, bool IsMobile, string PlatformName, string BrowserName)
        {
            // log to database
            m_cmdSQL = new SqlCommand
            {
                Connection = new SqlConnection(ConnectionString),
                CommandText = "", // your stored proc name
                CommandType = CommandType.StoredProcedure
            };
            m_cmdSQL.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            m_cmdSQL.Parameters.Add("@MailID", SqlDbType.Int).Value = MailID;
            m_cmdSQL.Parameters.Add("@Hash", SqlDbType.Char, 40).Value = Hash;
            m_cmdSQL.Parameters.Add("@IsMobile", SqlDbType.Bit).Value = IsMobile;
            m_cmdSQL.Parameters.Add("@IPAddress", SqlDbType.NVarChar, 64).Value = IPAddress;
            m_cmdSQL.Parameters.Add("@PlatformName", SqlDbType.NVarChar, 64).Value = PlatformName;
            m_cmdSQL.Parameters.Add("@BrowserName", SqlDbType.NVarChar, 64).Value = BrowserName;
            // open connection if necessary
            if (m_cmdSQL.Connection.State != ConnectionState.Open)
                m_cmdSQL.Connection.Open();
            return m_cmdSQL.BeginExecuteNonQuery(cb, extraData);
        }
    }
}