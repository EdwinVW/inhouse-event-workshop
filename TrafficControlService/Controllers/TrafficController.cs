namespace TrafficControlService.Controllers;

[ApiController]
[Route("")]
public class TrafficController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IVehicleStateRepository _vehicleStateRepository;
    private readonly IMeasurementsService _measurementsService;
    private readonly ILogger<TrafficController> _logger;
    private readonly int _sectionLengthInKm = 10;

    public TrafficController(
        ILogger<TrafficController> logger,
        HttpClient httpClient,
        IVehicleStateRepository vehicleStateRepository,
        IMeasurementsService measurementsService)
    {
        _logger = logger;
        _httpClient = httpClient;
        _vehicleStateRepository = vehicleStateRepository;
        _measurementsService = measurementsService;
    }

    [HttpPost("entrycam")]
    public async Task<ActionResult> VehicleEntry(VehicleRegistered msg)
    {
        try
        {
            // log entry
            _logger.LogInformation($"ENTRY detected in lane {msg.Lane} at {msg.Timestamp.ToString("hh:mm:ss")} " +
                $"of vehicle with license-number {msg.LicenseNumber}.");

            // store vehicle state
            var vehicleState = new VehicleState
            {
                LicenseNumber = msg.LicenseNumber,
                EntryTimestamp = msg.Timestamp
            };
            await _vehicleStateRepository.SaveVehicleStateAsync(vehicleState);

            return Ok();
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpPost("exitcam")]
    public async Task<ActionResult> VehicleExit(VehicleRegistered msg)
    {
        try
        {
            // get vehicle state
            var vehicleState = await _vehicleStateRepository.GetVehicleStateAsync(msg.LicenseNumber);
            if (!vehicleState.HasValue)
            {
                return NotFound();
            }

            // update state
            vehicleState = vehicleState.Value with { ExitTimestamp = msg.Timestamp };
            await _vehicleStateRepository.SaveVehicleStateAsync(vehicleState.Value);

            // calculate average speed
            double elapsedMinutes = vehicleState.Value.ExitTimestamp.Value
                .Subtract(vehicleState.Value.EntryTimestamp).TotalSeconds; // 1 sec. == 1 min. in simulation
            int avgSpeedInKmh = (int)Math.Round((_sectionLengthInKm / elapsedMinutes) * 60);

            // log exit
            _logger.LogInformation($"EXIT detected in lane {msg.Lane} at {msg.Timestamp.ToString("hh:mm:ss")} " +
                $"of vehicle with license-number {msg.LicenseNumber}. Average speed: {avgSpeedInKmh} Kmh.");

            // publish average speed
            var averageSpeedmeasured = new AverageSpeedMeasured(avgSpeedInKmh, msg.Timestamp);
            await _measurementsService.SendAverageSpeedMeasuredAsync(averageSpeedmeasured);

            return Ok();
        }
        catch
        {
            return StatusCode(500);
        }
    }
}
