using AutoMapper;
using FluentValidation.AspNetCore;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using Serilog;
using Topluluk.Services.AuthenticationAPI.Model.Mapper;
using Topluluk.Services.AuthenticationAPI.Model.Validators;
using Topluluk.Services.AuthenticationAPI.Services.Core;
using Topluluk.Shared.Helper;
using Topluluk.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Mapper Configuration

var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new GeneralMapper());
});
builder.Services.AddSingleton(mapperConfig.CreateMapper());
IConfiguration configuration = builder.Configuration;

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(configuration.GetSection("RabbitMQ:Host").Value), host =>
        {
            host.Username(configuration.GetSection("RabbitMQ:Username").Value);
            host.Password(configuration.GetSection("RabbitMQ:Password").Value);
        });

    });
});

builder.Services.AddMassTransitHostedService();

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));


// Service registration
builder.Services.AddInfrastructure();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Validation Configuration
builder.Services.AddControllers(options => options.Filters.Add<ValidationHelper>())
    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateUserValidator>())
    .ConfigureApiBehaviorOptions(options => options.SuppressConsumesConstraintForFormFileParameters = true);



builder.Host.UseSerilog();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.UseSerilogRequestLogging();
app.MapControllers();
app.UseMiddleware<RequestResponseMiddleware>();

app.UseCors();
app.Run();
Log.CloseAndFlush();
