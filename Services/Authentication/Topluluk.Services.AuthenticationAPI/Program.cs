using AutoMapper;
using FluentValidation.AspNetCore;
using MassTransit;
using Topluluk.Services.AuthenticationAPI.Model.Mapper;
using Topluluk.Services.AuthenticationAPI.Model.Validators;
using Topluluk.Services.AuthenticationAPI.Services.Core;
using Topluluk.Shared.Helper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Mapper Configuration

var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new GeneralMapper());
});
builder.Services.AddSingleton(mapperConfig.CreateMapper());

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost","/", host =>
        {
            host.Username("guest");
            host.Password("guest");
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

