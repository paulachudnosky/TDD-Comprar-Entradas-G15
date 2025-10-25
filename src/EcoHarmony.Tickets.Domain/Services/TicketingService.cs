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

        // --- CAMBIO: Precios actualizados ---
        private const decimal PRICE_REG = 5000m;
        private const decimal PRICE_VIP = 10000m;

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

            // El total ahora usa los precios actualizados (5k/10k)
            decimal total = request.Visitors.Sum(CalculatePrice);

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

            // 1. Conteo por Tipo de Pase
            int regularCount = request.Visitors.Count(v => v.PassType == PassType.Regular);
            int vipCount = request.Visitors.Count(v => v.PassType == PassType.Vip);

            // --- NUEVO: 2. Conteo por Rango de Edad ---
            int menores3 = request.Visitors.Count(v => v.Age <= 3);
            int menores15 = request.Visitors.Count(v => v.Age >= 4 && v.Age <= 15);
            int adultos = request.Visitors.Count(v => v.Age >= 16 && v.Age <= 59);
            int mayores60 = request.Visitors.Count(v => v.Age >= 60);

            // 3. Asunto y Pago (sin cambios)
            var subject = $"Confirmación de tu compra para EcoHarmony Park (Fecha: {request.VisitDate:dd/MM/yyyy})";

            var paymentInfo = request.PaymentMethod == PaymentMethod.Card
                ? $"Se ha procesado un pago de <strong>{total:C} {request.Currency}</strong> con tu tarjeta."
                : $"Por favor, recuerda abonar <strong>{total:C} {request.Currency}</strong> en la boletería del parque el día de tu visita.";

            // 4. Construcción del HTML del email
            var htmlBody = $$"""
                <html>
                <head>
                    <style>
                        body { font-family: Arial, sans-serif; line-height: 1.6; }
                        .container { width: 90%; margin: auto; padding: 20px; }
                        .header { font-size: 24px; color: #134611; }
                        .content { margin-top: 20px; }
                        ul { list-style-type: none; padding-left: 0; }
                        li { font-size: 16px; margin-bottom: 10px; }
                        strong { color: #3E8914; }
                        .small-list { list-style-type: disc; margin-left: 25px; }
                        .small-list li { font-size: 14px; margin-bottom: 5px; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1 class='header'>¡Tu visita a EcoHarmony Park está confirmada!</h1>
                        
                        <p>Hola {{request.BuyerEmail}},</p>
                        <p>Gracias por elegirnos. Aquí están los detalles de tu compra:</p>
                        <hr>
                        <p><strong>Fecha de visita:</strong> {{request.VisitDate:dddd, dd 'de' MMMM 'de' yyyy}}</p>
                        <p><strong>Monto Total:</strong> {{total:C}} {{request.Currency}}</p>
                        <p><strong>Total de Entradas:</strong> {{request.Visitors.Count}}</p>

                        
                        <p><strong>Detalle de pases:</strong></p>
                        <ul class="small-list">
                            <li>Entradas Regulares: <strong>{{regularCount}}</strong></li>
                            <li>Entradas VIP: <strong>{{vipCount}}</strong></li>
                        </ul>

                        <p><strong>Desglose por edad:</strong></p>
                        <ul class="small-list">
                            <li>Menores (0-3 años): <strong>{{menores3}}</strong> (No pagan)</li>
                            <li>Menores (4-15 años): <strong>{{menores15}}</strong> (Pagan 50%)</li>
                            <li>Adultos (16-59 años): <strong>{{adultos}}</strong> (Pagan 100%)</li>
                            <li>Mayores (60+ años): <strong>{{mayores60}}</strong> (Pagan 50%)</li>
                        </ul>
                        <hr>
                        <p>{{paymentInfo}}</p>
                        <br>
                        <p>¡Te esperamos para que disfrutes de una experiencia inolvidable!</p>
                        <p><em>El equipo de EcoHarmony Park</em></p>
                    </div>
                </body>
                </html>
            """;

            // 5. Envía el nuevo email
            _email.Send(request.BuyerEmail, subject, htmlBody);
            
            // --- FIN DE LA MEJORA DEL EMAIL ---

            return result;
        }

        private decimal CalculatePrice(Visitor visitor)
        {
            // Esta lógica ahora usa los nuevos precios (5k/10k)
            decimal basePrice = visitor.PassType == PassType.Vip ? PRICE_VIP : PRICE_REG;
            
            if (visitor.Age <= 3) return 0;
            if (visitor.Age >= 4 && visitor.Age <= 15) return Math.Round(basePrice / 2);
            if (visitor.Age >= 16 && visitor.Age <= 59) return basePrice;
            if (visitor.Age >= 60) return Math.Round(basePrice / 2);
            
            return basePrice; // Caso por defecto (si la edad es 0 o nula)
        }

        private void Validate(PurchaseRequest req)
        {
            // ... (Tu lógica de validación no necesita cambios) ...
            if (!_users.Exists(req.UserId))
                throw new BusinessRuleException("Se debe permitir la compra solo a usuario registrado.");

            if (req.Visitors == null || req.Visitors.Count == 0)
                throw new BusinessRuleException("Debe indicar la cantidad de entradas y datos de visitantes.");

            if (req.Visitors.Count > 10)
                throw new BusinessRuleException("La cantidad de entradas no debe ser mayor a 10.");

            if (req.Visitors.Any(v => v.Age < 0))
                throw new BusinessRuleException("La edad de todos los visitantes debe ser un numero positivo.");

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
