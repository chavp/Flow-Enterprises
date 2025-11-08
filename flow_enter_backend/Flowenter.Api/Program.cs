using Flowenter.Api.Extensions;
using Flowenter.Api.Middleware;
using Flowenter.Api.Services;
using Flowenter.Domain.Models;
using Flowenter.Parties.IServices;
using Flowenter.Parties.Services;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog logging
builder.AddSerilogLogging();

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

builder.Services.Configure<TenantPartiesConnectionStrings>(options =>
    builder.Configuration.GetSection(nameof(TenantPartiesConnectionStrings)).Bind(options)
);

// Add database
builder.AddDatabase();

// Add Domain Services
builder.Services.AddScoped<IPartiesServices, PartiesServices>();
builder.Services.AddScoped<IContactMechanismServices, ContactMechanismServices>();

builder.Services.AddControllers()
    .AddNewtonsoftJson();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(
    options =>
    {
        options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
    });


var app = builder.Build();

app.UseErrorHandling();

// Use your custom middleware
app.UseMiddleware<CustomHeaderMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // https://localhost:7062/swagger/index.html
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Opent API v1");
        
    });

    // https://localhost:7062/api-docs/index.html
    app.UseReDoc(options =>
    {
        options.SpecUrl("/openapi/v1.json");
    });

    // https://localhost:7062/scalar/v1
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
