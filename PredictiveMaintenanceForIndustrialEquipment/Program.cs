using PredictiveMaintenanceForIndustrialEquipment.Services;

var builder = WebApplication.CreateBuilder(args);

// Register the PredictionService as a singleton with dependency injection.
// The PredictionService will pull the PostgreSQL connection string from configuration.
builder.Services.AddSingleton<PredictionService>();

// Add controllers to the service container.
builder.Services.AddControllers();

var app = builder.Build();

// Map controller routes.
app.MapControllers();

// Run the application.
app.Run();