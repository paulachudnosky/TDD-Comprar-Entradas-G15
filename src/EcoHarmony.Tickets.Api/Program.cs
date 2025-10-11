using EcoHarmony.Tickets.Domain.Entities;
using EcoHarmony.Tickets.Domain.Ports;
using EcoHarmony.Tickets.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Swagger/OpenAPI básico
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adapters "dummy" para demo
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepo>();
builder.Services.AddSingleton<IParkCalendar, SimpleCalendar>();
builder.Services.AddSingleton<IEmailSender, ConsoleEmailSender>();
builder.Services.AddSingleton<IPaymentGateway, FakePaymentGateway>();

builder.Services.AddScoped<ITicketingService, TicketingService>();

var app = builder.Build();

// Swagger UI por defecto
app.UseSwagger();
app.UseSwaggerUI();

// Endpoint principal
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

app.MapGet("/", () => Results.Redirect("/swagger"));
app.Run();


// --------- Implementaciones demo ----------
class InMemoryUserRepo : IUserRepository
{
    public bool Exists(Guid userId) => true; // demo: todos existen
}

class SimpleCalendar : IParkCalendar
{
    // Abre de miércoles a domingo
    public bool IsOpen(DateOnly date)
    {
        var d = date.DayOfWeek;
        return d == DayOfWeek.Wednesday || d == DayOfWeek.Thursday ||
               d == DayOfWeek.Friday || d == DayOfWeek.Saturday ||
               d == DayOfWeek.Sunday;
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
