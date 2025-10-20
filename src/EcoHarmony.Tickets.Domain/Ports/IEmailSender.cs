namespace EcoHarmony.Tickets.Domain.Ports
{
    public interface IEmailSender
    {
        void Send(string to, string subject, string body);
    }
}
