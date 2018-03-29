using System.Configuration;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Configuration;


namespace Microsoft.Bot.Sample.ProactiveBot
{
    public class EmailSender
    {
        private static string apiKey = ConfigurationManager.AppSettings["SendGridKey"];
        public static async Task<bool> SendEmail(string body, string recipient, string sender, string subject)
        {
            /*try
            {
                dynamic sg = new SendGridAPIClient(apiKey);
                Email from = new Email(sender);
                Email to = new Email(recipient);
                Content content = new Content("text/plain", body);
                Mail mail = new Mail(from, subject, to, content);

                dynamic response = await sg.client.mail.send.post(requestBody: mail.Get());
                return true;
            }*/
            /*catch (Exception)
            {
                return false;
            }*/
            return false;
        }
    }
}
