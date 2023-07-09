using Billing.Interfaces;
using Billing.Services;
using FarmerMarket.API.Core;
using Microsoft.Extensions.Caching.Memory;
using Offers.Interfaces;
using Offers.Services;
using Product.Interfaces;
using Product.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// In-Memory Caching
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IBillingService, BillingService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOfferService, OfferService>();
var app = builder.Build();
ContextService.MemoryCache = app.Services.GetRequiredService<IMemoryCache>();
ContextService.BuildCache();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
