using System;

namespace EcoHarmony.Tickets.Domain.Services
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}
