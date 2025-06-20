using System.Net;
using System.Net.Mail;
using User_Auth_WebApp.Repository.Interface;
using User_Auth_WebApp.ViewModel.Email;

namespace User_Auth_WebApp.Repository.Service
{
    public class EmailSernder : IEmailSender
    {
        private readonly IConfiguration configuration;

        public EmailSernder(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<bool> EmailSendAsync(string email, string Subject, string message)
        {
            bool status = false;
            try
            {
                GetEmailSetting getEmailSetting = new GetEmailSetting()
                {
                    SecretKet = configuration.GetValue<string>("AppSettings:SercretKey"),
                    From = configuration.GetValue<string>("AppSettings:EmailSettings:From"),
                    SmtpServer = configuration.GetValue<string>("AppSettings:EmailSettings:SmtpServer"),
                    Port = configuration.GetValue<int>("AppSettings:EmailSettings:Port"),
                    EnableSSL = configuration.GetValue<bool>("AppSettings:EmailSettings:EnableSSL"),
                };
                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(getEmailSetting.From),
                    Subject = Subject,
                    Body = message
                };
                mailMessage.To.Add(email);
                SmtpClient smtpClient = new SmtpClient(getEmailSetting.SmtpServer)
                {
                    Port = getEmailSetting.Port,
                    Credentials = new NetworkCredential(getEmailSetting.From,getEmailSetting.SecretKet),
                    EnableSsl = getEmailSetting.EnableSSL
                };
                await smtpClient.SendMailAsync(mailMessage);
                status = true;
            }
            catch (Exception ex) 
            {
                status = false;
            }
            return status;
        }
    }
}
