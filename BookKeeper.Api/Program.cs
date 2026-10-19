using BookKeeper.Core;
using BookKeeper.Core.Repository;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using BookKeeper.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<BookKeeperDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IBookRepository, EfBookRepository>();
builder.Services.AddScoped<BookService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
