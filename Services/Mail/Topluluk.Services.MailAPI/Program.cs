using MassTransit;
using Topluluk.Services.MailAPI.Services.Consumers;
using Topluluk.Services.MailAPI.Services.Consumers.Authentication;
using Topluluk.Services.MailAPI.Services.Consumers.Event;
using Topluluk.Services.MailAPI.Services.Implementation;
using Topluluk.Services.MailAPI.Services.Interface;
using Topluluk.Shared.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMailService, MailService>();

IConfiguration configuration = builder.Configuration;

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SuccessfullyRegisteredConsumer>();
    x.AddConsumer<ResetPasswordConsumer>();
    x.AddConsumer<EventDeletedCosumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(configuration.GetSection("RabbitMQ:Host").Value), host =>
        {
            host.Username(configuration.GetSection("RabbitMQ:Username").Value);
            host.Password(configuration.GetSection("RabbitMQ:Password").Value);
        });

        cfg.ReceiveEndpoint("successfully-registered", e =>
        {
            e.ConfigureConsumer<SuccessfullyRegisteredConsumer>(context);
        });
        cfg.ReceiveEndpoint("reset-password", e =>
        {
            e.ConfigureConsumer<ResetPasswordConsumer>(context);
        });
        cfg.ReceiveEndpoint("event-deleted", e =>
        {
            e.ConfigureConsumer<EventDeletedCosumer>(context);
        });
    });
});

builder.Services.AddMassTransitHostedService();

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
app.UseCors();
app.Run();

