using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using ViewModel.Model;

namespace Common.Extentions
{
    public class EmailExtention
    {
        private IConfiguration _config;

        public EmailExtention(IConfiguration config)
        {
            _config = config;
        }

        public string SendEmail(EmailViewModel emailModel)
        {
            MailUser user = new MailUser();
            using (var message = new MailMessage())
            {
                foreach (var email in emailModel.ToEmailList)
                {
                    user.Name = email.Name;
                    user.EmailId = email.EmailId;
                    message.To.Add(new MailAddress(user.EmailId, "To" + user.Name));
                }
                message.From = new MailAddress("test.demo@app.com", "App Test");
                message.Subject = emailModel.Subject;
                message.Body = emailModel.Content;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient(_config["Email:Host"]))
                {
                    client.Host = _config["Email:Host"];
                    client.Port = Convert.ToInt32(_config["Email:Port"]);
                    client.EnableSsl = Convert.ToBoolean(_config["Email:EnableSsl"]);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = Convert.ToBoolean(_config["Email:UseDefaultCredentials"]);
                    client.Credentials = new NetworkCredential(_config["Email:UserEmail"], _config["Email:UserPassword"]);
                    client.Send(message);
                    message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

                    return message.DeliveryNotificationOptions.ToString();
                }
            }
        }

    }
}
