using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.Caching;
using System.Drawing;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Text.RegularExpressions;
namespace BaseClasses
{
    public class BaseFunctions
    {
        static private BaseFunctions theInstance;

        static public BaseFunctions getInstance()
        {
            if (BaseFunctions.theInstance == null)
                BaseFunctions.theInstance = new BaseFunctions();
            return BaseFunctions.theInstance;
        }


        public string GetDataResource(string UICulture, string resourceName, string resourceQualifier)
        {
            string result = "";

            if (string.IsNullOrEmpty(UICulture))
            {
                if (BaseDB.SessionContext.Current == null)
                    UICulture = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
                else
                    UICulture = BaseDB.SessionContext.Current.UICulture;
            }

            DataSet ds = BaseDB.CacheManager.GetDataSetFromCache("BaseDataResource", "Select * From BaseDataResource");
            DataView dw = ds.Tables[0].DefaultView;

            if (!string.IsNullOrEmpty(resourceQualifier))
            {
                dw.RowFilter = String.Format("UICulture='{0}' and ResourceName='{1}.{2}'",
                    UICulture, resourceQualifier, resourceName);
                if (dw.Count > 0)
                    return (string)dw[0]["Resource"];
                //else normaline bakacak, qualifier olmadan
            }

            dw.RowFilter = "UICulture='" + UICulture + "' And ResourceName='" + resourceName + "'";

            if (dw.Count == 0)
                result = resourceName;
            else
                result = (string)dw[0]["Resource"];

            return result;
        }

        public string GetDataResource(string UICulture, string ResourceName)
        {
            return GetDataResource(UICulture, ResourceName, null);
        }

        public string GetAlertResource(string UICulture, string alertId)
        {
            string result = "";

            if (string.IsNullOrEmpty(UICulture))
                UICulture = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();

            DataSet ds = BaseDB.CacheManager.GetDataSetFromCache("BaseAlertResource", "Select * From BaseAlertResource");
            DataView dw = ds.Tables[0].DefaultView;
            dw.RowFilter = "UICulture='" + UICulture + "' And ResourceId='" + alertId + "'";

            if (dw.Count == 0)
                result = alertId;
            else
                result = (string)dw[0]["Resource"];

            return result;
        }

        public string GetApplicationParameterValue(string section, string parameterName)
        {
            return BaseDB.DBManager.AppConnection.ExecuteSql("select ParameterValue from BaseApplicationParameters where Section=@section and ParameterName=@parameterName", new ArrayList { "section", "parameterName" }, new ArrayList { section, parameterName });
        }


        public DataSet FillDataWithResource(DataSet ds, string tableName, string UICulture)
        {
            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    dc.Caption = GetDataResource(UICulture, dc.ColumnName, tableName);
                }
            }
            return ds;
        }

        public DataSet FillDataWithResource(DataSet ds, string UICulture)
        {
            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    dc.Caption = GetDataResource(UICulture, dc.ColumnName);
                }
            }
            return ds;
        }

        public string GetSessionValue(string sessionName)
        {
            string session = "";
            if (HttpContext.Current.Session[sessionName] != null)
                session = HttpContext.Current.Session[sessionName].ToString();
            return session;
        }

        public void SetSessionValue(string sessionName, object sessionValue)
        {
            HttpContext.Current.Session[sessionName] = sessionValue;
        }

        public void ShowAlert(string UICulture, string message)
        {
            try
            {
                Page page = HttpContext.Current.CurrentHandler as Page;
                BaseFunctions functions = new BaseFunctions();

                string cleanMessage = message.Replace("'", "\\'");
                cleanMessage = functions.GetAlertResource(UICulture, cleanMessage);

                string script = "<script language=javascript>alert(\"" + HttpContext.Current.Server.HtmlDecode(cleanMessage) + "\")</script>";

                if (page != null)
                {
                    //page.RegisterClientScriptBlock("Alert", script);
                    page.ClientScript.RegisterClientScriptBlock(page.GetType(), "Alert", script);
                }
            }
            catch (Exception exp)
            {

            }
        }

        public string GetRandomPassword(int length)
        {
            Random rand = new Random();
            System.Text.StringBuilder password = new System.Text.StringBuilder(length);
            for (int i = 1; i <= length; i++)
            {
                int charIndex;
                // allow only digits and letters
                do
                {
                    charIndex = rand.Next(48, 123);
                } while (!((charIndex >= 48 && charIndex <= 57) ||
                (charIndex >= 65 && charIndex <= 90) || (charIndex >= 97 && charIndex <= 122)));


                // add the random char to the password being built
                password.Append(Convert.ToChar(charIndex));

            }
            return password.ToString();
        }

        public bool IsNumeric(string theValue)
        {
            try
            {
                Convert.ToDecimal(theValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string FillStringWithSpecifiedCharacter(string str, string character, int length)
        {
            while (str.Length < length)
            {
                str = character + str;
            }

            return str;
        }

        public void GetChildRecords(DataTable dtAll, DataTable dtChild, string keyField, string parentField, string recordId)
        {
            dtChild.Rows.Add(dtAll.Select(keyField + " = " + recordId)[0].ItemArray);
            foreach (DataRow dr in dtAll.Select(parentField + " = " + recordId))
            {
                GetChildRecords(dtAll, dtChild, keyField, parentField, dr[keyField].ToString());
            }
        }

        public void GetChildRecords(DataTable dtAll, DataTable dtChild, string keyField, string parentField, Guid recordId)
        {
            dtChild.Rows.Add(dtAll.Select(keyField + " = " + recordId)[0].ItemArray);
            foreach (DataRow dr in dtAll.Select(parentField + " = " + recordId))
            {
                GetChildRecords(dtAll, dtChild, keyField, parentField, dr[keyField].ToString());
            }
        }

        public void SendSMTPMail(string to, string cc, string subject, string body, string reply_to, System.Net.Mail.Attachment[] att, string img_path, string tip)
        {
            SMTPMail smtp_mail = new SMTPMail();
            smtp_mail.SendMail(to, cc, subject, body, reply_to, att, img_path, tip);
        }

       

        private class SMTPMail
        {
            private string to = "";
            private string subject = "";
            private string body = "";
            private string cc = "";
            private string reply_to = "";
            private System.Net.Mail.Attachment[] attacments = null;
            private string imgpath = "";
            private string tip = "";
            
            public void SendMail(string mailTo, string mailCc, string mailSubject, string mailBody, string replyto, System.Net.Mail.Attachment[] atts, string img_path, string tip)
            {
                SMTPMail mail = new SMTPMail();
                mail.to = mailTo;
                mail.subject = mailSubject;
                mail.body = mailBody;
                mail.cc = mailCc;
                mail.reply_to = replyto;
                mail.attacments = atts;
                mail.imgpath = img_path;
                mail.tip = tip;
                
                System.Threading.ThreadPool.SetMaxThreads(20, 0);
                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(mail.SendMail));
            }
            private void SendMail(object o)
            {
                try
                {
                    System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                    if (to.Trim() == "") return;

                    if (!to.Contains('@'))
                        return;
                    else
                        if (to.Split('@')[1].Trim() == "") return;

                    Thread.Sleep(1000);
                    
                    if (tip == "genel")
                    {
                        if (System.Configuration.ConfigurationSettings.AppSettings["MailAccount"] == null || System.Configuration.ConfigurationSettings.AppSettings["MailAccount"].ToString().Trim() == "" || System.Configuration.ConfigurationSettings.AppSettings["MailFromAddress"] == null || System.Configuration.ConfigurationSettings.AppSettings["MailFromAddress"].Trim() == "")
                            return;

                        msg.From = new MailAddress(System.Configuration.ConfigurationSettings.AppSettings["MailFromAddress"],
                                                System.Configuration.ConfigurationSettings.AppSettings["MailFromName"]);
                    }

                    msg.To.Add(to.Replace(";", ","));

                    if (reply_to != "") msg.ReplyToList.Add(reply_to);

                    if (cc != "" && cc.Contains('@'))
                        msg.CC.Add(cc.Replace(";", ","));

                    msg.Subject = subject;
                    msg.Body = body;
                    msg.IsBodyHtml = true;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;

                    AlternateView plainView = AlternateView.CreateAlternateViewFromString
                   (System.Text.RegularExpressions.Regex.Replace(body, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                    System.Net.Mail.AlternateView htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                    msg.AlternateViews.Add(plainView);
                    msg.AlternateViews.Add(htmlView);

                    if (attacments != null)
                    {
                        foreach (System.Net.Mail.Attachment atc in attacments)
                        {
                            msg.Attachments.Add(atc);
                        }
                    }

                    if (imgpath != "")
                    {
                        System.Net.Mail.LinkedResource imageResourceEs = new System.Net.Mail.LinkedResource(imgpath, "image/jpg");
                        imageResourceEs.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                        imageResourceEs.ContentId = "companylogo";

                        AlternateView av1 = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
                        av1.LinkedResources.Add(imageResourceEs);

                        msg.AlternateViews.Add(av1);
                    }

                    try
                    {
                        if (System.Configuration.ConfigurationSettings.AppSettings["SMTPMailServer"] != "" && System.Configuration.ConfigurationSettings.AppSettings["SMTPMailServerPort"] != "")
                        {
                            System.Net.Mail.SmtpClient c = new SmtpClient(System.Configuration.ConfigurationSettings.AppSettings["SMTPMailServer"], int.Parse(System.Configuration.ConfigurationSettings.AppSettings["SMTPMailServerPort"]));
                            if (tip == "genel")
                            {
                                c.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationSettings.AppSettings["MailAccount"],
                                        System.Configuration.ConfigurationSettings.AppSettings["MailAccountPwd"]);
                                c.EnableSsl = true;
                            }
                            //c.ServicePoint.ConnectionLimit = 10;
                            c.Send(msg);

                        }
                    }
                    catch (Exception ex)
                    {
                       // BaseDB.DBManager.AppConnection.ExecuteSql("insert into log_mail values('" + to + "','" + msg.From.Address + "','" + gonderen_aktif_user_name + "','" + tip + "','" + subject + "','" + ex.Message + "',getdate())");
                        return;
                    }
                }
                catch (Exception exp)
                {
                    return;
                }
            }
        }

        public Control FindControlReqursive(string controlId, Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                Control result = FindControlReqursive(controlId, control);
                if (result != null)
                {
                    return result;
                }
            }
            return parent.FindControl(controlId);
        }

        public Control FindControlInRepeater(string controlId, Repeater rpt)
        {
            Control cnt = null;
            foreach (RepeaterItem item in rpt.Items)
            {
                Control c = item.FindControl("div_soru");

                if (c != null)
                {
                    cnt = FindControlReqursive(controlId, c);
                    if (cnt != null)
                        break;
                }
            }

            return cnt;
        }

        public bool IsEmailValid(string email)
        {
            bool result = false;

            string patternStrict = "^([a-zA-Z0-9_\\-\\.]+)@[a-z0-9-]+(\\.[a-z0-9-]+)*(\\.[a-z]{2,3})$";

            Regex reStrict = new Regex(patternStrict);
            result = reStrict.IsMatch(email);
            return result;
        
        }
       
    }
}
