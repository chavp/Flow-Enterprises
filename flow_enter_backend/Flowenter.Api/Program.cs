using Flowenter.Api.Extensions;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add database services
builder.Services.AddDatabaseServices(builder.Configuration);

builder.Services.AddControllers()
    .AddNewtonsoftJson();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(
    options =>
    {
        options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
    });

var app = builder.Build();

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
