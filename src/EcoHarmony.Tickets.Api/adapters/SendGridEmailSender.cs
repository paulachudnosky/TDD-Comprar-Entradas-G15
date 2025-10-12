using EcoHarmony.Tickets.Domain.Ports;
using Microsoft.Extensions.Configuration; // Necesario para IConfiguration
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace EcoHarmony.Tickets.Api.Adapters
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public SendGridEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async void Send(string to, string subject, string body)
{
    Console.WriteLine(">>> [SendGridEmailSender]: El método Send() ha sido invocado.");

    var apiKey = _configuration["SendGrid:ApiKey"];
    if (string.IsNullOrEmpty(apiKey))
    {
        Console.WriteLine(">>> [SendGridEmailSender]: ERROR FATAL - La ApiKey de SendGrid no está configurada o es nula.");
        return;
    }

    Console.WriteLine(">>> [SendGridEmailSender]: ApiKey encontrada. Preparando para enviar.");
    
    var client = new SendGridClient(apiKey);

    // --- CAMBIOS BASADOS EN TU CAPTURA ---
    // 1. Se usa el email verificado exacto: "iswgrupo15@gmail.com".
    //    El nombre "Parque EcoHarmony" es lo que el usuario verá como remitente.
    var from = new EmailAddress("iswgrupo15@gmail.com", "Parque EcoHarmony");
    
    var toAddress = new EmailAddress(to);
    
    var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, "", body);

    // 2. (Opcional) Se configura el Reply-To para que coincida.
    var replyToAddress = new EmailAddress("iswgrupo15@gmail.com");
    msg.ReplyTo = replyToAddress;
    // --- FIN DE LOS CAMBIOS ---

    try
    {
        Console.WriteLine($">>> [SendGridEmailSender]: Enviando email a '{to}'...");
        var response = await client.SendEmailAsync(msg);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine(">>> [SendGridEmailSender]: ¡ÉXITO! El email fue aceptado por la API de SendGrid.");
        }
        else
        {
            Console.WriteLine($">>> [SendGridEmailSender]: ERROR - La API de SendGrid devolvió un error: {response.StatusCode}");
            Console.WriteLine(await response.Body.ReadAsStringAsync());
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($">>> [SendGridEmailSender]: EXCEPCIÓN CATASTRÓFICA al intentar enviar el email: {ex.Message}");
    }
}
    }
}
