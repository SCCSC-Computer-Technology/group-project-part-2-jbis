using System.Net;
using System.Net.Mail;

namespace WorldWideSportsApp.Helpers
{
    public static class EmailHelper
    {
        //Simple Mail Transfer Protocol - used to send emails from our app to the user's email address for verification purposes
        public static async Task SendVerificationEmailAsync(IConfiguration config, string toEmail, string code)
        {
            var smtpHost = config["Smtp:Host"];
            var smtpPort = int.Parse(config["Smtp:Port"]!);
            var smtpUser = config["Smtp:Username"];
            var smtpPass = config["Smtp:Password"];
            var fromEmail = config["Smtp:From"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };
            //the message and code to verify the user's email address, along with the expiration time of the code (15 minutes)
            var mail = new MailMessage(fromEmail!, toEmail)
            {
                Subject = "Your WorldWide Sports Verification Code",
                Body = $"Your verification code is: {code}\n\nThis code expires in 15 minutes.",
                IsBodyHtml = false
            };

            await client.SendMailAsync(mail);
        }
    }
}
