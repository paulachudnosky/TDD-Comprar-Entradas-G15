namespace EcoHarmony.Tickets.Domain.Entities
{
    public enum PassType { Regular, Vip }

    public class Visitor
    {
        public int Age { get; set; }
        public PassType PassType { get; set; }
        public decimal Price { get; set; }

    }
}
