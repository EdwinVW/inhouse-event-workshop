var httpClient = new HttpClient();
var measurementsService = new HttpMeasurementsService(httpClient);
var maximumSpeed = args.Length == 1 ? Convert.ToInt32(args[0]) : 100;
var sensor = new Sensor(maximumSpeed, measurementsService);
await sensor.Start();
Task.Run(() => Thread.Sleep(Timeout.Infinite)).Wait();
