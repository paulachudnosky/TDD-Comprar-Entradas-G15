using EcoHarmony.Tickets.Domain.Entities;

namespace EcoHarmony.Tickets.Domain.Services
{
    public interface ITicketingService
    {
        PurchaseResult BuyTickets(PurchaseRequest request);
    }
}
