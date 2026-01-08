using EmailTriageAgent.Application;
using EmailTriageAgent.Application.Runners;
using EmailTriageAgent.Application.Services;
using EmailTriageAgent.Infrastructure;
using EmailTriageAgent.ML;
using EmailTriageAgent.Web.Workers;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EmailTriageDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<QueueService>();
builder.Services.AddScoped<ScoringService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<TrainingService>();
builder.Services.AddScoped<SettingsService>();
builder.Services.AddScoped<EmailQueryService>();
builder.Services.AddScoped<DecisionRules>();
builder.Services.AddScoped<ScoringAgentRunner>();
builder.Services.AddScoped<RetrainAgentRunner>();
builder.Services.AddSingleton<IEmailClassifier, KeywordEmailClassifier>();

builder.Services.AddHostedService<ScoringWorker>();
builder.Services.AddHostedService<RetrainWorker>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EmailTriageDbContext>();
    await DatabaseSeeder.SeedAsync(dbContext, CancellationToken.None);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();
app.Run();
