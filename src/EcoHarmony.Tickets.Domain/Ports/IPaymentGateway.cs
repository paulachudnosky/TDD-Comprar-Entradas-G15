namespace EcoHarmony.Tickets.Domain.Ports
{
    public interface IPaymentGateway
    {
        string CreatePayment(decimal amount, string currency);
    }
}
