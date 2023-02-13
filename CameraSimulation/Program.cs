var maximumSpeed = args.Length == 1 ? Convert.ToInt32(args[0]) : 100;
if (maximumSpeed < 80 || maximumSpeed > 130)
{
    Console.WriteLine($"Error: maximum speed must be between 80 and 130");
    return;
}
Console.WriteLine($"Maximum speed is {maximumSpeed}");

int lanes = 3;
Camera[] cameras = new Camera[lanes];
var httpClient = new HttpClient();
for (var i = 0; i < lanes; i++)
{
    int camNumber = i + 1;
    var trafficControlService = new HttpTrafficControlService(httpClient);
    cameras[i] = new Camera(camNumber, maximumSpeed, trafficControlService);
}
Parallel.ForEach(cameras, cam => cam.Start());

Task.Run(() => Thread.Sleep(Timeout.Infinite)).Wait();