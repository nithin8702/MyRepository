using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using University.Security.Models.ViewModel;

namespace University.Api.Controllers
{
    public class EmailController : UnSecuredController
    {
        public HttpResponseMessage Post(ApiViewModel apiViewModel)
        {
            _logger.Info("Email");
            SmtpClient smtpMail = new SmtpClient();
            MailMessage mailMessage = new MailMessage();
            try
            {
                //fromAddress = scheduler.EmailFrom;
                //toAddress = scheduler.EmailTo;
                //ccAddress = scheduler.EmailCC;
                //port = scheduler.Port.Value;
                //hostName = scheduler.EmailSchedulerHostName;
                smtpMail.Port = 25;
                smtpMail.Host = "alinsaaf@ip-104-238-116-102.secureserver.net";// "smtpout.secureserver.net";
                smtpMail.EnableSsl = false;// scheduler.EnableSsl.Value;
                smtpMail.UseDefaultCredentials = false;
                smtpMail.Credentials = new NetworkCredential("ameer", "Ammer123!");
                mailMessage.From = (new MailAddress("test@al-insaaf.com"));
                mailMessage.To.Add(new MailAddress("nithin8702@gmail.com"));
                mailMessage.IsBodyHtml = true;
                mailMessage.Subject = "Test Mail";
                mailMessage.Body = "Hello World";
                smtpMail.Send(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return null;
        }

    }
}
