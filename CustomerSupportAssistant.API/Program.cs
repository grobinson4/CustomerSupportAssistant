using CustomerSupportAssistant.Persistence;
using CustomerSupportAssistant.Persistence.Seed;
using CustomerSupportAssistant.Business.Services;
using CustomerSupportAssistant.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using CustomerSupportAssistant.Persistence.Repositories;
using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Azure;
using Azure.AI.OpenAI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.MigrationsAssembly("CustomerSupportAssistant.Persistence")));

// Repositories
// Add these registrations
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
builder.Services.AddScoped<InquiryProcessorService>();
builder.Services.AddSingleton<OpenAIService>();
builder.Services.AddLogging();
builder.Services.AddSingleton(_ =>
{
    var endpoint = new Uri(builder.Configuration["AzureOpenAI:Endpoint"]);
    var key = new AzureKeyCredential(builder.Configuration["AzureOpenAI:Key"]);
    return new OpenAIClient(endpoint, key);
});


// Other services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Optional: Keep PascalCase if needed
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Customer Support Assistant API", Version = "v1" });

    // 🔥 Add OAuth2 Configuration
    options.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.OAuth2,
        Flows = new Microsoft.OpenApi.Models.OpenApiOAuthFlows
        {
            AuthorizationCode = new Microsoft.OpenApi.Models.OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    { $"{builder.Configuration["AzureAd:Audience"]}/access_as_user", "Access API on behalf of signed-in user" }
                }
            }
        }
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { $"{builder.Configuration["AzureAd:Audience"]}/access_as_user" }
        }
    });
}

);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteDev", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // Important if using auth tokens
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        await DbInitializer.SeedAsync(dbContext);
    }
    catch (Exception ex)
    {
        // Handle exceptions (e.g., log them)
        Console.WriteLine($"Seeding error: {ex.Message}");
    }
}

// Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer Support Assistant API V1");

    options.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
    options.OAuthUsePkce(); // Use Proof Key for Code Exchange (mandatory now)
    options.OAuthScopeSeparator(" ");
    options.OAuthScopes($"{builder.Configuration["AzureAd:Audience"]}/access_as_user");
});
}
app.UseCors("AllowViteDev");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
