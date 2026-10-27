using BookKeeper.Core;
using BookKeeper.Core.Repository;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using BookKeeper.Infrastructure;
using BookKeeper.Api.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

// The API stores the books in PostgreSQL through EF Core. The connection string comes from Docker Compose
// (ConnectionStrings__DefaultConnection) or from user-secrets when the API runs with dotnet run.
// Scoped: one DbContext per request, so the repository and the service are scoped too.
builder.Services.AddDbContext<BookKeeperDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IBookRepository, EfBookRepository>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddScoped<AuthService>();

// GET /health is Healthy only when the API can reach the database.
builder.Services.AddHealthChecks()
    .AddDbContextCheck<BookKeeperDbContext>();

var app = builder.Build();

// Creates or updates the tables when the API starts. Only Docker Compose turns this on (Database__MigrateOnStartup),
// so the tests and dotnet run never touch the database schema.
if (app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<BookKeeperDbContext>().Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.AddPreferredSecuritySchemes("Bearer"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
