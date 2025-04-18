using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace ModelLayer.Models
{
    public class Send
    {
        public string SendMail(string ToEmail, string Token)
        {
            string FromEmail = "hariprasadreddy413381@gmail.com";
            MailMessage message = new MailMessage(FromEmail, ToEmail);
            string MailBody = "The token for the reset password: " + Token; 
            message.Subject = "Token generated for resetting password";
            message.Body = MailBody.ToString();
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            NetworkCredential credential = new NetworkCredential("hariprasadreddy413381@gmail.com", "ilhc axdk dxgq njhh");

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = credential;
            smtpClient.Send(message);

            return ToEmail;
        }
    }
}
