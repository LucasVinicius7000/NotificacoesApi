using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Notificacoes.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


string? connectionString = builder.Configuration.GetConnectionString("ServiceBusNamespace")?.ToString();
builder.Services.AddScoped<IMessageService<ServiceBusMessage, ServiceBusReceivedMessage>, MessageService>(provider =>
{
    return new MessageService(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
