using Application;
using Domain.CustomExceptions;
using Infrastrucutre;
using Presentation;
using Serilog;
using System.Net;
using WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddApplication()
    .AddInfrastructure()
    .AddPresentation();

builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapGet("/badrequest", () =>
{
    throw new BusinessException("Bad Request", HttpStatusCode.BadRequest, "Required request body");
});

app.MapGet("/unauthorized", () =>
{
    throw new BusinessException("Unauthorized", HttpStatusCode.Unauthorized, "Wrong credentials!");
});

app.MapGet("/servererror", () =>
{
    throw new Exception();
});

app.Run();
