using Olava.Api.Security;
using Olava.Application.Abstractions;
using Olava.Infrastructure;
using Olava.Infrastructure.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ── Framework services ──
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();

// ── Per-request identity & tenant (one object, two interfaces) ──
builder.Services.AddScoped<CurrentUser>();
builder.Services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<CurrentUser>());
builder.Services.AddScoped<ITenantProvider>(sp => sp.GetRequiredService<CurrentUser>());

// ── Our layers ──
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication();
builder.Services.AddAppAuthorization();
builder.Services.AddApiCors(builder.Configuration);
builder.Services.AddAppRateLimiting();

var app = builder.Build();

// Roles are seeded always; migrations auto-apply in Development only (prod: run them in CI/CD).
await app.Services.InitialiseDatabaseAsync(applyMigrations: app.Environment.IsDevelopment());

app.UseExceptionHandler();      // turns unhandled exceptions into RFC 9457 problem details
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();   // /openapi/v1.json
    app.MapScalarApiReference().AllowAnonymous();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health").AllowAnonymous();
app.MapControllers().RequireAuthorization(); ;

app.Run();

public partial class Program;