using AutoMapper;
using Topluluk.Services.CommunityAPI.Data.Settings;
using Topluluk.Services.CommunityAPI.Model.Mapper;
using Topluluk.Services.CommunityAPI.Services.Core;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http.Features;

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



builder.Services.AddControllers();
builder.Services.Configure<FormOptions>(o =>  // currently all set to max, configure it to your needs!
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = long.MaxValue; // <-- !!! long.MaxValue
    o.MultipartBoundaryLengthLimit = int.MaxValue;
    o.MultipartHeadersCountLimit = int.MaxValue;
    o.MultipartHeadersLengthLimit = int.MaxValue;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
//app.Use(async (context, next) =>
//{
//    context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = null; // unlimited I guess
//    await next.Invoke();
//});
app.Run();

