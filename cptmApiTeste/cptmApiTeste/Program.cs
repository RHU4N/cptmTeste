using cptmApiTeste.Domain.Model.InspecaoAggregate;
using cptmApiTeste.Infraestrutura;
using cptmApiTeste.Infraestrutura.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ConectContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=localhost:1521/XEPDB1;User ID=RHUAN;Password=root"));

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyPolicy",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:5173",
                    "http://localhost:4173",
                    "https://localhost:5173",
                    "https://localhost:4173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddScoped<IInspecaoRepository, InspecaoRepository>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ConectContext>();
    context.Database.Migrate();
}

app.UseCors("MyPolicy");

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
