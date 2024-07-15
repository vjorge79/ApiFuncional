using ApiFuncional.Configuration;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApiConfig()
    .AddCorsConfig()
    .AddSwaggerConfig()
    .AddDbContextConfig()
    .AddIdentityConfig();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("Development");
}
else
{
    app.UseCors("Production");
}

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

        if (exceptionHandlerFeature != null)
        {
            var ex = exceptionHandlerFeature.Error;

            var logger = factory.CreateLogger("ErrorHandler");
            logger.LogError($"Error: {ex}");

            var problem = new ProblemDetails()
            {
                Type = ex.GetType().Name,
                Status = context.Response.StatusCode,
                Instance = exceptionHandlerFeature.Path,
                Title = app.Environment.IsDevelopment() ? $"{ex.Message}" : "An error ocurred.",
                Detail = app.Environment.IsDevelopment() ? ex.StackTrace : null,
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    });
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();