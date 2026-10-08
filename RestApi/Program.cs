using CoreBase.Database;
using Microsoft.EntityFrameworkCore;
using RestApi;
using RestApi.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
  options.AddPolicy("web", policy =>
    policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
      .AllowAnyHeader()
      .AllowAnyMethod());
});
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

app.UseCors("web");
app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();
