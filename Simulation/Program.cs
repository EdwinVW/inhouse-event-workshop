// determine max speed
var maximumSpeed = args.Length == 1 ? Convert.ToInt32(args[0]) : 120;
int[] allowedMaxSpeeds = new int[] { 80, 90, 100, 120, 130 };
if (!allowedMaxSpeeds.Contains(maximumSpeed))
{
    Console.WriteLine($"Error: maximum speed must be 80, 90, 100, 120 or 130");
    return;
}
Console.WriteLine($"Maximum speed is {maximumSpeed}");

Console.WriteLine("Starting simulation. Press any key to exit.");

CancellationTokenSource ctSource = new CancellationTokenSource();

// start cameras
int lanes = 3;
var httpClient = new HttpClient();
for (var i = 0; i < lanes; i++)
{
    int camNumber = i + 1;
    var trafficControlService = new HttpTrafficControlService(httpClient);
    var camera = new Camera(camNumber, maximumSpeed, trafficControlService);
    camera.Start(ctSource.Token);
}

// start CO2 level sensor
var measurementsService = new HttpMeasurementsService(httpClient);
var sensor = new Sensor(maximumSpeed, measurementsService);
sensor.Start(ctSource.Token);

Console.ReadKey(true);

ctSource.Cancel();