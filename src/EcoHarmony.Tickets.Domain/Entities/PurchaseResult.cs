namespace EcoHarmony.Tickets.Domain.Entities
{
    public class PurchaseResult
    {
        public bool Success { get; set; }
        public int TicketsCount { get; set; }
        public DateOnly VisitDate { get; set; }
        public string? PaymentRedirectUrl { get; set; }
        public bool PayAtTicketOffice { get; set; }
        public string ConfirmationMessage { get; set; } = string.Empty;
    }
}
