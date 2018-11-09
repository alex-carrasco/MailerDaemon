using System;
using System.Net.Mail;
using System.Text;
using System.Web;
using Microsoft.VisualBasic;

namespace MailTrack
{
    public class NotificationManager
    {
        public static void Send(string SMTPServer, string FromAddress, string ToAddress, string Subject, string Body)
        {
            Send(SMTPServer, FromAddress, ToAddress, Subject, Body, false, null);
        }
        public static void Send(string SMTPServer, string FromAddress, string ToAddress, string Subject, string Body, bool IsBodyHtml)
        {
            Send(SMTPServer, FromAddress, ToAddress, Subject, Body, IsBodyHtml, null);
        }

        public static void Send(string SMTPServer, string FromAddress, string ToAddress, string Subject, string Body, HttpRequest HttpRequest)
        {
            Send(SMTPServer, FromAddress, ToAddress, Subject, Body, false, HttpRequest);
        }

        public static void Send(string SMTPServer, string FromAddress, string ToAddress, string Subject, string Body, bool IsBodyHtml, HttpRequest HttpRequest)
        {
            Send(SMTPServer, FromAddress, ToAddress, Subject, Body, IsBodyHtml, HttpRequest, true, DateTime.MinValue);
        }

        public static void Send(string SMTPServer, string FromAddress, string ToAddress, string Subject, string Body, bool IsBodyHtml, HttpRequest HttpRequest, bool Verbose, DateTime RequestStartTime)
        {
            string EOL;
            StringBuilder sb = new StringBuilder();

            if (IsBodyHtml)
            {
                EOL = "<br>";
                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine("<style type='text/css'>");
                sb.AppendLine(" BODY {");
                sb.AppendLine("     font-family: Verdana, Helvetica, sans-serif;");
                sb.AppendLine("     font-size: 9pt;");
                sb.AppendLine(" }");
                sb.AppendLine(" TD, TH {");
                sb.AppendLine("     background-color: White;");
                sb.AppendLine("     font-size: 8pt;");
                sb.AppendLine(" }");
                sb.AppendLine(" TH {");
                sb.AppendLine("     text-align: right;");
                sb.AppendLine(" }");
                sb.AppendLine(" TD.TableHeader {");
                sb.AppendLine("     background-color: WhiteSmoke;");
                sb.AppendLine("     font-family: Verdana, Helvetica, sans-serif;");
                sb.AppendLine("     font-size: 13pt;");
                sb.AppendLine("     font-weight: bolder;");
                sb.AppendLine(" }");
                sb.AppendLine(" TD.TableSectionHeader {");
                sb.AppendLine("     background-color: #ededed;");
                sb.AppendLine("     font-style: italic;");
                sb.AppendLine(" }");
                sb.AppendLine("</style>");
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");
            }
            else
                EOL = string.Empty;
            sb.AppendLine(Body);
            if (HttpRequest != null)
            {
                string Browser = "[null]";
                string Referrer = "[null]";
                {
                    if (IsBodyHtml)
                    {
                        sb.AppendLine("<br><br>");
                        sb.AppendLine("<table border='1'>");
                        sb.AppendLine("<tr><td colspan='2' class='TableHeader'>Incoming Request To Our Page</td></tr>");
                        sb.AppendLine("<tr><td colspan='2' class='TableSectionHeader'>Request</td></tr>");
                        sb.AppendLine("<tr><th>URL:</th><td><a href='" + HttpRequest.Url.ToString() + "'>" + HttpRequest.Url.ToString() + "</a></td></tr>");
                        sb.Append("<tr><th>Time (UTC):</th><td>");
                        if (RequestStartTime == DateTime.MinValue)
                            sb.Append("[unknown]");
                        else
                            sb.Append(RequestStartTime.ToString());
                        sb.AppendLine("</td></tr>");
                        sb.AppendLine("<tr><th>Type:</th><td>" + HttpRequest.RequestType + "</td></tr>");
                        sb.AppendLine("<tr><th>Client IP:</th><td>" + HttpRequest.UserHostAddress + "</td></tr>");
                        if (HttpRequest.UrlReferrer != null)
                            Referrer = HttpRequest.UrlReferrer.ToString();
                        sb.AppendLine("<tr><th>Referrer:</th><td>" + Referrer + "</td></tr>");
                        sb.AppendLine("<tr><th>User Agent:</th><td>" + HttpRequest.UserAgent + "</td></tr>");
                        sb.AppendLine("<tr><th>Email Send Time (UTC):</th><td>" + DateTime.UtcNow.ToString() + "</td></tr>");
                        if (Verbose)
                        {
                            if (HttpRequest.Browser.Browser != null)
                                Browser = HttpRequest.Browser.Browser;
                            sb.AppendLine("<tr><td colspan='2' class='TableSectionHeader'>Browser</td></tr>");
                            sb.AppendLine("<tr><th>Browser Type:</th><td>" + Browser + "</td></tr>");
                            sb.AppendLine("<tr><th>Crawler?:</th><td>" + HttpRequest.Browser.Crawler + "</td></tr>");
                            sb.AppendLine("<tr><th>Supports Frames?:</th><td>" + HttpRequest.Browser.Frames + "</td></tr>");
                            sb.AppendLine("<tr><th>Supports Tables?:</th><td>" + HttpRequest.Browser.Frames + "</td></tr>");
                            sb.AppendLine("<tr><th>Supports Cookies?:</th><td>" + HttpRequest.Browser.Cookies + "</td></tr>");
                            sb.AppendLine("<tr><th>Supports CSS?:</th><td>" + HttpRequest.Browser.SupportsCss + "</td></tr>");
                            sb.AppendLine("<tr><th>ECMAScript Version:</th><td>" + HttpRequest.Browser.EcmaScriptVersion.ToString() + "</td></tr>");
                            sb.AppendLine("<tr><th>MSHTML DOM Version:</th><td>" + HttpRequest.Browser.MSDomVersion.ToString() + "</td></tr>");
                            sb.AppendLine("<tr><th>XML DOM Version:</th><td>" + HttpRequest.Browser.W3CDomVersion.ToString() + "</td></tr>");
                            sb.AppendLine("<tr><th>Platform:</th><td>" + HttpRequest.Browser.Platform + "</td></tr>");
                        }
                        if (HttpRequest.Headers.Count > 0)
                        {
                            sb.AppendLine("<tr><td colspan='2' class='TableSectionHeader'>Header Key/Values</td></tr>");
                            foreach (var key in HttpRequest.Headers.AllKeys)
                                sb.AppendLine("<tr><th>" + key + ":</th><td>" + HttpRequest.Headers.Get(key) + "</td></tr>");
                        }
                        if (HttpRequest.RequestType == "POST")
                        {
                            sb.AppendLine("<tr><td colspan='2' class='TableSectionHeader'>Form Key/Values</td></tr>");
                            if (HttpRequest.Form.HasKeys())
                            {
                                foreach (var key in HttpRequest.Form.AllKeys)
                                    sb.AppendLine("<tr><th>" + key + ":</th><td>" + HttpRequest.Form.Get(key) + "</td></tr>");
                            }
                            else
                                sb.AppendLine("<tr><th>No form values present!</th><td></td></tr>");
                        }
                        sb.AppendLine("</table><br>");
                    }
                    else
                    {
                        sb.AppendLine(EOL);
                        sb.AppendLine("-- Request --------------------------------------------" + EOL);
                        sb.AppendLine("URL: " + HttpRequest.Url.ToString() + EOL);
                        sb.Append("Time (UTC): ");
                        if (RequestStartTime == DateTime.MinValue)
                            sb.Append("[unknown]");
                        else
                            sb.Append(RequestStartTime.ToString());
                        sb.AppendLine(EOL);
                        sb.AppendLine("Type: " + HttpRequest.RequestType + EOL);
                        sb.AppendLine("Client IP: " + HttpRequest.UserHostAddress + EOL);
                        if (HttpRequest.UrlReferrer != null)
                            Referrer = HttpRequest.UrlReferrer.ToString();
                        sb.AppendLine("Referrer: " + Referrer + EOL);
                        sb.AppendLine("Email Send Time (UTC):" + DateTime.UtcNow.ToString() + EOL);
                        if (Verbose == true)
                        {
                            sb.AppendLine("Content Length: " + HttpRequest.ContentLength + EOL);
                            sb.AppendLine("Application Path: " + HttpRequest.ApplicationPath + EOL);
                            sb.AppendLine("Path: " + HttpRequest.Path + EOL);
                            sb.AppendLine("Agent: " + HttpRequest.UserAgent + EOL);
                            sb.AppendLine("-- Browser --------------------------------------------" + EOL);
                            if (HttpRequest.Browser.Browser != null)
                                Browser = HttpRequest.Browser.Browser;
                            sb.AppendLine("Browser String: " + Browser + EOL);
                            sb.AppendLine("Crawler: " + HttpRequest.Browser.Crawler + EOL);
                            sb.AppendLine("Frames: " + HttpRequest.Browser.Frames + EOL);
                            sb.AppendLine("Tables: " + HttpRequest.Browser.Tables + EOL);
                            sb.AppendLine("ECMAScript Version: " + HttpRequest.Browser.EcmaScriptVersion.ToString() + EOL);
                            sb.AppendLine("MSHTML DOM Version: " + HttpRequest.Browser.MSDomVersion.ToString() + EOL);
                            sb.AppendLine("XML DOM Version: " + HttpRequest.Browser.W3CDomVersion.ToString() + EOL);
                            sb.AppendLine("Platform: " + HttpRequest.Browser.Platform + EOL);
                            sb.AppendLine("Win16 Platform: " + HttpRequest.Browser.Win16 + EOL);
                            sb.AppendLine("Win32 Platform: " + HttpRequest.Browser.Win32 + EOL);
                        }
                        if (HttpRequest.Headers.Count > 0)
                        {
                            sb.AppendLine("-- Headers --------------------------------------------" + EOL);
                            foreach (var key in HttpRequest.Headers.AllKeys)
                                sb.AppendLine(key + ControlChars.Tab + HttpRequest.Headers.Get(key) + EOL);
                        }
                        if (HttpRequest.Cookies.Count > 0)
                        {
                            sb.AppendLine("-- Cookies --------------------------------------------" + EOL);
                            foreach (var key in HttpRequest.Cookies.AllKeys)
                                sb.AppendLine(key + ControlChars.Tab + HttpRequest.Cookies.Get(key).Value + EOL);
                        }
                        if (HttpRequest.Form.HasKeys())
                        {
                            sb.AppendLine("-- Form Key/Values ------------------------------------" + EOL);
                            foreach (var key in HttpRequest.Form.AllKeys)
                                sb.AppendLine(key + ControlChars.Tab + HttpRequest.Form.Get(key) + EOL);
                        }
                        if (HttpRequest.QueryString.HasKeys())
                        {
                            sb.AppendLine("-- QueryString Key/Values -----------------------------" + EOL);
                            foreach (var key in HttpRequest.QueryString.AllKeys)
                                sb.AppendLine(key + ControlChars.Tab + HttpRequest.QueryString.Get(key) + EOL);
                        }
                        sb.AppendLine("-------------------------------------------------------" + EOL);
                    }
                }
            }
            if (IsBodyHtml)
            {
                sb.AppendLine("</body>");
                sb.AppendLine("</html>");
            }

            SmtpClient EmailClient = new SmtpClient(SMTPServer);
            MailMessage EmailMessage = new MailMessage(FromAddress, ToAddress, Subject, sb.ToString())
            {
                IsBodyHtml = IsBodyHtml
            };

            try
            {
                EmailClient.Send(EmailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending notification email. " + ex.Message);
            }
        }
    }
}