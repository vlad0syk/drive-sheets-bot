using GoogleDriveWebhook.Services;
using GoogleDriveWebhook.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var googleSettings = new GoogleSettings();
builder.Configuration.GetSection("Google").Bind(googleSettings);
builder.Services.AddSingleton(googleSettings);

builder.Services.AddSingleton<GoogleDriveService>();
builder.Services.AddSingleton<GoogleSheetsService>();
builder.Services.AddSingleton<FolderScanner>();
builder.Services.AddSingleton<DeadlineUpdater>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
