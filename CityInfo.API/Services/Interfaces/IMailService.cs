namespace CityInfo.API.Services.Interfaces
{
    public interface IMailService
    { 
        public void Send(string subject, string body);
    }
}
