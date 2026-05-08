using cptmApiTeste.Domain.Model.InspecaoAggregate;
using cptmApiTeste.Domain.Model.UsuarioAggregate;
using cptmApiTeste.Infraestrutura;
using cptmApiTeste.Infraestrutura.Repositories;
using cptmApiTeste.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "cptm-api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "cptm-web";
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "MinhaChaveSecretaMuitoForteESegura12345";
var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

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
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = jwtKey,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ConectContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();
    context.Database.Migrate();

    context.Database.ExecuteSqlRaw(@"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM USER_TAB_COLUMNS
    WHERE TABLE_NAME = 'INSPECAO' AND COLUMN_NAME = 'USUARIO_ID';

    IF v_count = 0 THEN
        EXECUTE IMMEDIATE 'ALTER TABLE ""INSPECAO"" ADD ""USUARIO_ID"" NUMBER(10)';
    END IF;
END;");

    context.Database.ExecuteSqlRaw(@"
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM USER_INDEXES
    WHERE INDEX_NAME = 'IX_INSPECAO_USUARIO_ID';

    IF v_count = 0 THEN
        EXECUTE IMMEDIATE 'CREATE INDEX ""IX_INSPECAO_USUARIO_ID"" ON ""INSPECAO"" (""USUARIO_ID"")';
    END IF;
END;");

    var users = context.Usuarios.ToList();
    var usersUpdated = false;

    foreach (var user in users)
    {
        if (HasIdentityPasswordHash(user.password))
        {
            continue;
        }

        if (string.IsNullOrWhiteSpace(user.password))
        {
            continue;
        }

        user.AtualizarSenha(passwordHasher.HashPassword(user, user.password));
        usersUpdated = true;
    }

    var operadorExists = context.Usuarios.Count(x => x.username == "operador") > 0;
    if (!operadorExists)
    {
        var operador = new Usuario("operador", passwordHasher.HashPassword(null!, "operador123"), "user");
        context.Usuarios.Add(operador);
        usersUpdated = true;
    }

    if (usersUpdated)
    {
        context.SaveChanges();
    }
}

app.UseCors("MyPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static bool HasIdentityPasswordHash(string value)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        return false;
    }

    return value.StartsWith("AQAAAA", StringComparison.Ordinal)
        && Regex.IsMatch(value, "^[A-Za-z0-9+/=]+$");
}
