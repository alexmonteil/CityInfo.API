using CityInfo.API.Services.Interfaces;

namespace CityInfo.API.Services
{
    public class DefaultMailService : IMailService
    {
        private readonly string _mailFrom = string.Empty;
        private readonly string _mailTo = string.Empty;

        public DefaultMailService(IConfiguration configuration)
        {
            _mailTo = configuration["mailSettings:mailToAddress"];
            _mailFrom = configuration["mailSettings:mailFromAddress"];
        }
        
        public void Send(string subject, string body)
        {
            // mock sending an email by writing to the console
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with {nameof(DefaultMailService)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {body}");
        }
    }
}
