using CoreBase.Database;
using Microsoft.EntityFrameworkCore;
using RestApi;
using RestApi.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigDbContext(builder.Configuration);
builder.Services.AddAppModule();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  var userDb = services.GetRequiredService<UserDbContext>();
  await userDb.Database.MigrateAsync();
  await UserSeed.RunAsync(userDb);
}

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();
