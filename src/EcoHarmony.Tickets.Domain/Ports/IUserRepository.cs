using System;

namespace EcoHarmony.Tickets.Domain.Ports
{
    public interface IUserRepository
    {
        bool Exists(Guid userId);
    }
}
