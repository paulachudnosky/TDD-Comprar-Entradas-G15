using System;
using System.Collections.Generic;

namespace EcoHarmony.Tickets.Domain.Entities
{
    public enum PaymentMethod { Unspecified = 0, Cash = 1, Card = 2 }

    public class PurchaseRequest
    {
        public Guid UserId { get; set; }
        public DateOnly VisitDate { get; set; }
        public List<Visitor> Visitors { get; set; } = new List<Visitor>();
        public PaymentMethod PaymentMethod { get; set; }
        public string BuyerEmail { get; set; } = string.Empty;
        public string Currency { get; set; } = "ARS";
    }
}
