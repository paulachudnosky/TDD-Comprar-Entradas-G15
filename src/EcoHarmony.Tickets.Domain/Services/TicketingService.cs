using System;
using System.Linq;
using EcoHarmony.Tickets.Domain.Entities;
using EcoHarmony.Tickets.Domain.Ports;

namespace EcoHarmony.Tickets.Domain.Services
{
    public class TicketingService : ITicketingService
    {
        private readonly IUserRepository _users;
        private readonly IParkCalendar _calendar;
        private readonly IEmailSender _email;
        private readonly IPaymentGateway _payments;

        public TicketingService(IUserRepository users, IParkCalendar calendar, IEmailSender email, IPaymentGateway payments)
        {
            _users = users;
            _calendar = calendar;
            _email = email;
            _payments = payments;
        }

        public PurchaseResult BuyTickets(PurchaseRequest request)
        {
            Validate(request);

            // Precios demo (placeholder)
            decimal total = request.Visitors.Sum(v => v.PassType == PassType.Vip ? 25000m : 15000m);

            string? redirect = null;
            bool payAtCounter = false;

            if (request.PaymentMethod == PaymentMethod.Card)
            {
                redirect = _payments.CreatePayment(total, request.Currency);
            }
            else if (request.PaymentMethod == PaymentMethod.Cash)
            {
                payAtCounter = true;
            }

            var result = new PurchaseResult
            {
                Success = true,
                TicketsCount = request.Visitors.Count,
                VisitDate = request.VisitDate,
                PaymentRedirectUrl = redirect,
                PayAtTicketOffice = payAtCounter,
                ConfirmationMessage = $"Compra confirmada: {request.Visitors.Count} entradas para {request.VisitDate}.",
                TotalAmount = total,
                Currency = request.Currency
            };

            // --- INICIA LA MEJORA DEL EMAIL ---

            // 1. Define un asunto claro
            var subject = $"Confirmación de tu compra para EcoHarmony Park (Fecha: {request.VisitDate:dd/MM/yyyy})";

            // 2. Crea un mensaje de pago dinámico
            var paymentInfo = request.PaymentMethod == PaymentMethod.Card
                ? $"Se ha procesado un pago de <strong>{total:C} {request.Currency}</strong> con tu tarjeta. Serás redirigido para finalizar."
                : $"Por favor, recuerda abonar <strong>{total:C} {request.Currency}</strong> en la boletería del parque el día de tu visita.";

            // 3. Construye el cuerpo del email con HTML
            var htmlBody = $"""
                <html>
                <body style="font-family: sans-serif; padding: 20px;">
                    <h1 style="color: #2E7D32;">¡Tu visita a EcoHarmony Park está confirmada!</h1>
                    <p>Hola,</p>
                    <p>Gracias por elegirnos. Aquí están los detalles de tu compra:</p>
                    <hr>
                    <p><strong>Número de entradas:</strong> {request.Visitors.Count}</p>
                    <p><strong>Fecha de visita:</strong> {request.VisitDate:dddd, dd 'de' MMMM 'de' yyyy}</p>
                    <p><strong>Monto Total:</strong> {total:C} {request.Currency}</p>
                    <hr>
                    <p>{paymentInfo}</p>
                    <br>
                    <p>¡Te esperamos para que disfrutes de una experiencia inolvidable!</p>
                    <p><em>El equipo de EcoHarmony Park</em></p>
                </body>
                </html>
            """;

            // 4. Envía el nuevo email
            _email.Send(request.BuyerEmail, subject, htmlBody);
            
            // --- FIN DE LA MEJORA DEL EMAIL ---

            return result;
        }

        private void Validate(PurchaseRequest req)
        {
            if (!_users.Exists(req.UserId))
                throw new BusinessRuleException("Se debe permitir la compra solo a usuario registrado.");

            if (req.Visitors == null || req.Visitors.Count == 0)
                throw new BusinessRuleException("Debe indicar la cantidad de entradas y datos de visitantes.");

            if (req.Visitors.Count > 10)
                throw new BusinessRuleException("La cantidad de entradas no debe ser mayor a 10.");

            if (req.Visitors.Any(v => v.Age <= 6))
                throw new BusinessRuleException("La edad de todos los visitantes debe ser mayor a 6 años.");

            var hoy = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            if (req.VisitDate < hoy)
                throw new BusinessRuleException("La fecha de visita debe ser el día actual o futuro.");

            if (!_calendar.IsOpen(req.VisitDate))
                throw new BusinessRuleException("La fecha de la visita debe estar dentro de los días en que el parque está abierto: el parque está cerrado.");

            if (req.PaymentMethod == PaymentMethod.Unspecified)
                throw new BusinessRuleException("Debe seleccionar la forma de pago (efectivo o tarjeta).");

            if (string.IsNullOrWhiteSpace(req.BuyerEmail))
                throw new BusinessRuleException("Debe ingresar un email para recibir la confirmación.");
        }
    }
}
