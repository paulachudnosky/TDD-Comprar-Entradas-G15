namespace EcoHarmony.Tickets.Domain.Ports
{
    public interface IParkCalendar
    {
        bool IsOpen(DateOnly date);
    }
}
