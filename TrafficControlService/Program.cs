// create web-app
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IVehicleStateRepository, InMemoryVehicleStateRepository>();
builder.Services.AddSingleton<IMeasurementsService, HttpMeasurementsService>();
builder.Services.AddControllers();

var app = builder.Build();

// configure web-app
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// configure routing
app.MapControllers();

// let's go!
app.Run("http://localhost:6000");
