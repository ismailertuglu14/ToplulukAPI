using AutoMapper;
using Topluluk.Services.CommunityAPI.Data.Settings;
using Topluluk.Services.CommunityAPI.Model.Mapper;
using Topluluk.Services.CommunityAPI.Services.Core;
using MongoDB.Driver;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new GeneralMapper());
});
builder.Services.AddSingleton(mapperConfig.CreateMapper());

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

builder.Services.AddCap(options =>
{
    options.UseMongoDB("mongodb+srv://ismail:ismail@cluster0.psznbcu.mongodb.net/?retryWrites=true&w=majority");
    options.UseRabbitMQ(options =>
    {
       
        options.ConnectionFactoryOptions = options =>
        {
            options.Ssl.Enabled = false;
            options.HostName = "localhost";
            options.UserName = "guest";
            options.Password = "guest";
            options.Port = 5672;
        };
    });
});

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
builder.Services.AddInfrastructure();

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

