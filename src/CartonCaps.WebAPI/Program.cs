using CartonCaps.Application.Repositories;
using CartonCaps.Infrastructure;
using CartonCaps.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MockDbContext>(options =>
{
    options.UseInMemoryDatabase("CartonCapsDatabase");
});

builder.Services.AddScoped<MockDbContext>();

builder.Services.AddControllers();

builder.Services.AddScoped<IReferralRepository, ReferralRepository>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CartonCaps API",
        Version = "v1",
        Description = "API for managing carton caps referrals.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Jess Annalise",
            Email = "jess@annagram.io",
            Url = new Uri("localhost:5155")
        }
    });
    options.EnableAnnotations();
});

var app = builder.Build();

app.UseSwagger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CartonCaps API V1");
        options.RoutePrefix = "swagger";
    });
}

app.UseCors();
app.MapControllers();

app.Run();