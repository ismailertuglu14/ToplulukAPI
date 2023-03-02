using AutoMapper;
using MongoDB.Driver;
using Topluluk.Services.User.Data.Settings;
using Topluluk.Services.User.Model.Mapper;
using Topluluk.Services.User.Services.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IMongoClient>(new MongoClient());

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
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure();
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new GeneralMapper());
});

builder.Services.AddSingleton(mapperConfig.CreateMapper());

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

