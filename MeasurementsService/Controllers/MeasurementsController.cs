namespace MeasurementsService.Controllers;

[ApiController]
[Route("")]
public class MeasurementsController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IMeasurementsRepository _repository;
    private readonly ILogger _logger;

    public MeasurementsController(
        ILogger<MeasurementsController> logger,
        HttpClient httpClient,
        IMeasurementsRepository measurementsRepository)
    {
        _logger = logger;
        _httpClient = httpClient;
        _repository = measurementsRepository;
    }

    [HttpPost("averageSpeed")]
    public async Task<ActionResult> AverageSpeedMeasured(AverageSpeedMeasured msg)
    {
        try
        {
            // log entry
            _logger.LogInformation($"Received Average Speed measurement of {msg.AverageSpeedInInKmh} Kmh at {msg.Timestamp.ToString("hh:mm:ss")} ");

            // store state
            var averageSpeed = new AverageSpeed
            {
                AverageSpeedInKmh = msg.AverageSpeedInInKmh,
                Timestamp = msg.Timestamp
            };
            await _repository.SaveAverageSpeedAsync(averageSpeed);

            return Ok();
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpPost("co2Level")]
    public async Task<ActionResult> CO2LevelMeasured(CO2LevelMeasured msg)
    {
        try
        {
            // log measurement
            _logger.LogInformation($"Received CO2 Level measurement of {msg.CO2LevelInPPM} PPM at {msg.Timestamp.ToString("hh:mm:ss")} ");

            // get average speeds during the last minute
            var averageSpeeds = await _repository.GetAverageSpeedsAsync(msg.Timestamp);
            int overallAverageSpeedInKmh = 0;
            if (averageSpeeds.Count() > 0)
            {
                overallAverageSpeedInKmh = (int)Math.Round(
                    averageSpeeds.Average(s => s.AverageSpeedInKmh), 
                    MidpointRounding.AwayFromZero);
            }

            // update state
            var samplePoint = new SamplePoint(overallAverageSpeedInKmh, msg.CO2LevelInPPM, msg.Timestamp);
            await _repository.SaveSamplePointAsync(samplePoint);

            // log sample point
            _logger.LogInformation($"Created sample point with overall average speed of {overallAverageSpeedInKmh}" +
                                   $" and CO2 level if {msg.CO2LevelInPPM} at {msg.Timestamp.ToString("hh:mm:ss")}");

            return Ok();
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpGet("metrics")]
    public async Task<IEnumerable<SamplePoint>> Metrics()
    {
        return await _repository.GetSamplePointsAsync();
    } 
}
