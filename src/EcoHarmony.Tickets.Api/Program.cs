using EcoHarmony.Tickets.Domain.Entities;
using EcoHarmony.Tickets.Domain.Ports;
using EcoHarmony.Tickets.Domain.Services;
using System.Text.Json.Serialization;
using EcoHarmony.Tickets.Api;
using EcoHarmony.Tickets.Api.Adapters;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------
//  🔧 CONFIGURACIÓN DE SERVICIOS
// ------------------------------------------------------

// Documentación Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS → permite que el frontend (Live Server o localhost) llame a la API
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(
                "http://localhost:8080",
                "http://127.0.0.1:5500",  // Live Server
                "http://localhost:5500",  // http-server
                "file://")                // apertura directa del HTML (algunos navegadores)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Inyección de dependencias (adapters falsos)
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepo>();
builder.Services.AddSingleton<IParkCalendar, SimpleCalendar>();
builder.Services.AddSingleton<IEmailSender, SendGridEmailSender>();   

builder.Services.AddSingleton<IPaymentGateway, FakePaymentGateway>();

// Servicio principal (dominio)
builder.Services.AddScoped<ITicketingService, TicketingService>();

// JSON options: strings para enums + DateOnly ISO
builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()); // <-- enums como "Card"/"Cash"/"Regular"/"Vip"
    o.SerializerOptions.Converters.Add(new DateOnlyJsonConverter());   // <-- si agregaste el conversor de DateOnly
});


var app = builder.Build();

// ------------------------------------------------------
//  🚀 MIDDLEWARES Y ENDPOINTS
// ------------------------------------------------------

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Activar CORS
app.UseCors();

// Endpoint principal de negocio
app.MapPost("/tickets/purchase", (PurchaseRequest req, ITicketingService service) =>
{
    try
    {
        var result = service.BuyTickets(req);
        return Results.Ok(result);
    }
    catch (BusinessRuleException bre)
    {
        return Results.BadRequest(new { error = bre.Message });
    }
});

// Redirección raíz → Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// ------------------------------------------------------
//  ▶️ EJECUCIÓN
// ------------------------------------------------------

// Si no se define otro puerto, arranca en 5080
var port = Environment.GetEnvironmentVariable("PORT") ?? "5080";
app.Urls.Add($"http://*:{port}");

app.Run();


// ------------------------------------------------------
//  🧩 ADAPTERS "DUMMY" PARA DEMO
// ------------------------------------------------------

class InMemoryUserRepo : IUserRepository
{
    public bool Exists(Guid userId) => true; // todos los usuarios existen
}

class SimpleCalendar : IParkCalendar
{
    // Abre de miércoles a domingo (demo)
    public bool IsOpen(DateOnly date)
    {
        var day = date.DayOfWeek;
        return day == DayOfWeek.Wednesday ||
               day == DayOfWeek.Thursday ||
               day == DayOfWeek.Friday ||
               day == DayOfWeek.Saturday ||
               day == DayOfWeek.Sunday;
    }
}

class ConsoleEmailSender : IEmailSender
{
    public void Send(string to, string subject, string body)
    {
        Console.WriteLine($"EMAIL -> {to} | {subject}\n{body}");
    }
}

class FakePaymentGateway : IPaymentGateway
{
    public string CreatePayment(decimal amount, string currency)
        => "https://sandbox.mercado-pago/checkout/demo-redirect";
}
