using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Data;
using Common;

namespace SendEmail
{
    class Program : CommonFunction
    {
      
        static void Main(string[] args)
        {
           
             try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("FROMXXXX@gmail.com");
                mail.To.Add("kyawlay707@gmail.com");
                mail.Subject = "Test Mail..!!!!";
                
                DataTable getmail = GetMailList();
                string strRowCommaSeparated = "";
                int presiteID = 0;
                int nextID = 0;
               for (int i = 0; i < getmail.Rows.Count; i++)
               {

                   nextID = Convert.ToInt32(getmail.Rows[i].ItemArray[2].ToString());

                   if (presiteID == nextID)
                   {
                       strRowCommaSeparated += "           - " + getmail.Rows[i].ItemArray[1].ToString() + " - " + getmail.Rows[i].ItemArray[0].ToString();
                       //strRowCommaSeparated + = "\nDescription - " + getmail.Rows[i].ItemArray[1].ToString() + " - " + getmail.Rows[i].ItemArray[0].ToString();
                   }
                   else
                   {
                       strRowCommaSeparated += "\nSiteID - " + getmail.Rows[i].ItemArray[2].ToString()+"\n";
                       strRowCommaSeparated += "          - " + getmail.Rows[i].ItemArray[1].ToString() + " - " + getmail.Rows[i].ItemArray[0].ToString();
                       strRowCommaSeparated += Environment.NewLine;
                   }

                  
                   presiteID = Convert.ToInt32(getmail.Rows[i].ItemArray[2].ToString());
                   mail.Body = strRowCommaSeparated;

               }

              

                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("testqbei@gmail.com", "admin_123456");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);

               }

             catch (Exception)
             { }
        }

        private static DataTable GetMailList()
        {
            throw new NotImplementedException();
        }
    }
}
