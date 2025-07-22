using System.Net;
using System.Net.Mail;

namespace RestaurantSystem.Services
{
    public class EmailSendService
    {
        private string smtp_host;
        private int smtp_port;
        private string smtp_fromEmail;
        private string smtp_password;

        public EmailSendService(IConfiguration configuration)
        {
            smtp_host = configuration.GetValue<string>("Smtp:Host", "smtp.gmail.com");
            smtp_port = configuration.GetValue<int>("Smtp:Port", 587);
            smtp_fromEmail = configuration.GetValue<string>("Smtp:From_Email", "");
            smtp_password = configuration.GetValue<string>("Smtp:App_Password", "");
        }

        public string GenerateVerificationCode(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        // noreply@savansrestaurant.com
        // Your Restaurant
        public bool SendEmailToClients(List<string> emails, string body_html, string subject)
        {
            using SmtpClient client = new SmtpClient()
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Host = smtp_host,
                Port = smtp_port,
                Credentials = new NetworkCredential(smtp_fromEmail, smtp_password)
            };

            bool res = false;
            foreach (string email in emails)
            {
                try
                {
                    client.Send(smtp_fromEmail, email, subject, body_html);
                    res = true;
                }
                catch 
                {
                }
            }
            return res;
        }
    }
}
