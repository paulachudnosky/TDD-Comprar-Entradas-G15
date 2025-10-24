namespace EcoHarmony.Tickets.Domain.Entities
{
    public enum PassType { Regular, Vip }

    public class Visitor
    {
        public int Age { get; set; }
        public PassType PassType { get; set; }
        // Calculated price for this visitor (ARS). Set by the backend when processing the purchase.
        public decimal Price { get; set; }
    }
}
