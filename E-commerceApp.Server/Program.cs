using Core.Interfaces;
using E_commerceApp.Server.Errors;
using E_commerceApp.Server.Helpers;
using E_commerceApp.Server.MiddleWare;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()  // Allow any HTTP method (GET, POST, etc.)
               .AllowAnyHeader());
});

// Dependency injection for repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// AutoMapper for DTO mappings
builder.Services.AddAutoMapper(typeof(MappingProfiles));

// Custom model validation error handling
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage).ToArray();
        var errorResponse = new ApiValidationErrorResponse
        {
            Errors = errors
        };
        return new BadRequestObjectResult(errorResponse);
    };
});

// Configure SQL Server connection
builder.Services.AddDbContext<StoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger documentation setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "E-commerce API", Version = "v1" });
});

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var env = services.GetRequiredService<IWebHostEnvironment>();

    try
    {
        var context = services.GetRequiredService<StoreContext>();
        await context.Database.MigrateAsync();
        await StoreContextSeed.SeedAsync(context, loggerFactory);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred during migration");
    }
}

// Middleware pipeline configuration
app.UseDefaultFiles();
app.UseStaticFiles(); // Ensure this is not duplicated

// CORS middleware should be used before authentication and authorization
app.UseCors("AllowAll");  // Apply CORS policy globally

app.UseMiddleware<ExceptionMiddleware>(); // Custom middleware for exception handling

app.UseStatusCodePagesWithReExecute("/errors/{0}"); // Custom error pages
app.UseHttpsRedirection();

// Routing and authentication middleware
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Swagger UI - available in development only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-commerce API v1");
    });
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();  // Map API controllers
});

// Fallback for single-page applications
app.MapFallbackToFile("/index.html");

app.Run();  // Run the application
