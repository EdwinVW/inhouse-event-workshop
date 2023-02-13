namespace CO2SensorSimulation;

public class Sensor
{
    private readonly IMeasurementsService _measurementsService;
    private readonly int _maximumSpeed;
    private readonly int _measurementIntervalInSec = 15;
    private Random _rnd;

    public Sensor(int maximumSpeed, IMeasurementsService measurementsService)
    {
        this._maximumSpeed = maximumSpeed;
        _measurementsService = measurementsService;
        _rnd = new Random();
    }

    public async Task Start()
    {
        Console.WriteLine($"Start CO2 Level simulation.");

        while (true)
        {
            try
            {
                Task.Delay(TimeSpan.FromSeconds(_measurementIntervalInSec)).Wait();

                // simulate measurement
                var co2LevelinPPM = GetCO2Level();
                var co2LevelMeasured = new CO2LevelMeasured(co2LevelinPPM, DateTime.Now);

                // log measurement
                Console.WriteLine($"Measured CO2 Level of {co2LevelinPPM} PPM at {co2LevelMeasured.Timestamp.ToString("hh:mm:ss")}");
                
                // publish measurement
                await _measurementsService.SendCO2LevelMeasuredAsync(co2LevelMeasured);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CO2 Level simulation error: {ex.Message}");
            }
        }
    }

    private int GetCO2Level()
    {
        // TODO: make dependent on max speed
        return _rnd.Next(250, 400);
    }
}
