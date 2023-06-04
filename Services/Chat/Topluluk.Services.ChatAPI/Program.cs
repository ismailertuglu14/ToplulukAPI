using AutoMapper;
using Topluluk.Services.ChatAPI.Model.Mapper;
using Topluluk.Services.ChatAPI.Services;
using Topluluk.Services.ChatAPI.Services.Core;
using Topluluk.Services.ChatAPI.Services.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new GeneralMapper());
});
builder.Services.AddSingleton(mapperConfig.CreateMapper());
builder.Services.AddInfrastructure();
builder.Services.AddSignalR();
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

app.UseCors("MyPolicy");


app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.MapHub<ChatHub>("/chat-hub");

app.Run();
