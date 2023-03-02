using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Topluluk.Services.FileAPI.Data.Settings;
using Topluluk.Services.FileAPI.Model.Mapper;
using Topluluk.Services.FileAPI.Services.Core;
using Topluluk.Services.FileAPI.Services.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new GeneralMapper());
});

builder.Services.AddSingleton(mapperConfig.CreateMapper());

builder.Services.AddInfrastructure();
builder.Services.AddStorage<AzureStorage>();

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

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

