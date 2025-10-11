using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using EcoHarmony.Tickets.Domain.Entities;
using EcoHarmony.Tickets.Domain.Ports;
using EcoHarmony.Tickets.Domain.Services;

namespace EcoHarmony.Tickets.Tests
{
    public class TicketingServiceTests
    {
        private TicketingService BuildService(
            bool userExists = true,
            bool dateIsOpen = true)
        {
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.Exists(It.IsAny<Guid>())).Returns(userExists);

            var calendar = new Mock<IParkCalendar>();
            calendar.Setup(c => c.IsOpen(It.IsAny<DateOnly>())).Returns(dateIsOpen);

            var email = new Mock<IEmailSender>();
            var pay = new Mock<IPaymentGateway>();
            pay.Setup(p => p.CreatePayment(It.IsAny<decimal>(), It.IsAny<string>()))
               .Returns("https://sandbox.mercado-pago/checkout/abc123");

            return new TicketingService(userRepo.Object, calendar.Object, email.Object, pay.Object);
        }

        private static PurchaseRequest ValidRequest(PaymentMethod method = PaymentMethod.Card, int qty = 2)
        {
            var visitors = new List<Visitor>();
            for (int i = 0; i < qty; i++)
            {
                visitors.Add(new Visitor { Age = 25, PassType = PassType.Regular });
            }

            return new PurchaseRequest
            {
                UserId = Guid.NewGuid(),
                VisitDate = DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(1),
                Visitors = visitors,
                PaymentMethod = method,
                BuyerEmail = "test@buyer.com"
            };
        }

        [Fact]
        public void Rejects_if_user_not_registered()
        {
            var service = BuildService(userExists: false);
            var req = ValidRequest();

            var ex = Assert.Throws<BusinessRuleException>(() => service.BuyTickets(req));
            Assert.Contains("usuario registrado", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Rejects_if_date_in_past()
        {
            var service = BuildService();
            var req = ValidRequest();
            req.VisitDate = DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(-1);

            var ex = Assert.Throws<BusinessRuleException>(() => service.BuyTickets(req));
            Assert.Contains("fecha", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Rejects_if_park_closed_that_day()
        {
            var service = BuildService(dateIsOpen: false);
            var req = ValidRequest();

            var ex = Assert.Throws<BusinessRuleException>(() => service.BuyTickets(req));
            Assert.Contains("parque está cerrado", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Rejects_if_quantity_over_10()
        {
            var service = BuildService();
            var req = ValidRequest(qty: 11);

            var ex = Assert.Throws<BusinessRuleException>(() => service.BuyTickets(req));
            Assert.Contains("no debe ser mayor a 10", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Rejects_if_missing_payment_method()
        {
            var service = BuildService();
            var req = ValidRequest();
            req.PaymentMethod = PaymentMethod.Unspecified;

            var ex = Assert.Throws<BusinessRuleException>(() => service.BuyTickets(req));
            Assert.Contains("forma de pago", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Accepts_card_payment_returns_redirect_and_sends_email()
        {
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.Exists(It.IsAny<Guid>())).Returns(true);

            var calendar = new Mock<IParkCalendar>();
            calendar.Setup(c => c.IsOpen(It.IsAny<DateOnly>())).Returns(true);

            var email = new Mock<IEmailSender>();

            var pay = new Mock<IPaymentGateway>();
            pay.Setup(p => p.CreatePayment(It.IsAny<decimal>(), It.IsAny<string>()))
               .Returns("https://sandbox.mercado-pago/checkout/xyz789");

            var service = new TicketingService(userRepo.Object, calendar.Object, email.Object, pay.Object);
            var req = ValidRequest(PaymentMethod.Card, qty: 3);

            var result = service.BuyTickets(req);

            Assert.True(result.Success);
            Assert.Equal(3, result.TicketsCount);
            Assert.Equal(req.VisitDate, result.VisitDate);
            Assert.Equal("https://sandbox.mercado-pago/checkout/xyz789", result.PaymentRedirectUrl);
            email.Verify(e => e.Send(req.BuyerEmail, It.Is<string>(s => s.Contains("confirmación", StringComparison.OrdinalIgnoreCase)),
                                     It.Is<string>(b => b.Contains("3 entradas") && b.Contains(result.VisitDate.ToString()))),
                         Times.Once);
        }

        [Fact]
        public void Accepts_cash_payment_without_redirect_and_sends_email()
        {
            var service = BuildService();
            var req = ValidRequest(PaymentMethod.Cash, qty: 1);

            var result = service.BuyTickets(req);

            Assert.True(result.Success);
            Assert.Equal(1, result.TicketsCount);
            Assert.Null(result.PaymentRedirectUrl);
            Assert.True(result.PayAtTicketOffice);
        }
    }
}
