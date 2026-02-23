using AISA.Application;
using AISA.Infrastructure;
using AISA.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ===== Servicii =====

// Application Layer (MediatR, FluentValidation, AutoMapper)
builder.Services.AddApplicationServices();

// Infrastructure Layer (EF Core, Repositories, AI Client)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AISA API",
        Version = "v1",
        Description = "AI Customer Sentiment Analyzer — RESTful API"
    });
});

// CORS (pt Blazor Frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetValue<string>("Frontend:Url") ?? "https://localhost:5010")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// ===== Creare automată tabele în DB =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AISA.Infrastructure.Persistence.AisaDbContext>();
    db.Database.EnsureCreated();
}

// ===== Middleware Pipeline =====

// Exception handling global
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger (disponibil mereu în development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AISA API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
