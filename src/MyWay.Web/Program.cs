using MyWay.Api.DependencyInjection;
using MyWay.Application.DependencyInjection;
using MyWay.EF.DependencyInjection;
using MyWay.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMyWayApi();
builder.Services.AddMyWayApplication();
builder.Services.AddMyWayEf(builder.Configuration);
builder.Services.AddMyWayInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
