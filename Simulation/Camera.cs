namespace Simulation;

public class Camera
{
    private readonly ITrafficControlService _trafficControlService;
    private Random _rnd;
    private int _camNumber;
    private int _minEntryDelayInMS = 50;
    private int _maxEntryDelayInMS = 5000;
    private int _minExitDelayInMs;
    private int _maxExitDelayInMs;

    public Camera(int camNumber, int maximumSpeed, ITrafficControlService trafficControlService)
    {
        _rnd = new Random();
        _camNumber = camNumber;
        CalculateExitDelays(maximumSpeed);
        _trafficControlService = trafficControlService;
    }

    public void Start(CancellationToken cancellationToken)
    {
        Task.Run(() => Loop(cancellationToken));
    }

    public void Loop(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Start camera {_camNumber} simulation.");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // simulate entry
                TimeSpan entryDelay = TimeSpan.FromMilliseconds(_rnd.Next(_minEntryDelayInMS, _maxEntryDelayInMS) + _rnd.NextDouble());
                Task.Delay(entryDelay).Wait();

                Task.Run(async () =>
                {
                    // simulate entry
                    DateTime entryTimestamp = DateTime.Now;
                    var vehicleRegistered = new VehicleRegistered
                    {
                        Lane = _camNumber,
                        LicenseNumber = GenerateRandomLicenseNumber(),
                        Timestamp = entryTimestamp
                    };
                    await _trafficControlService.SendVehicleEntryAsync(vehicleRegistered);
                    Console.WriteLine($"Simulated ENTRY of vehicle with license-number {vehicleRegistered.LicenseNumber} in lane {vehicleRegistered.Lane}");

                // simulate exit
                TimeSpan exitDelay = TimeSpan.FromMilliseconds(_rnd.Next(_minExitDelayInMs, _maxExitDelayInMs));
                    Task.Delay(exitDelay).Wait();
                    vehicleRegistered.Timestamp = DateTime.Now;
                    vehicleRegistered.Lane = _rnd.Next(1, 4);
                    await _trafficControlService.SendVehicleExitAsync(vehicleRegistered);
                    Console.WriteLine($"Simulated EXIT of vehicle with license-number {vehicleRegistered.LicenseNumber} in lane {vehicleRegistered.Lane}");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Camera {_camNumber} error: {ex.Message}");
            }
        }
    }

    #region Private helper methods

    private void CalculateExitDelays(int maximumSpeed)
    {
        double speedFactor = maximumSpeed / 100.0;
        double minDelayinMs = 6000 / speedFactor - 400;
        double maxDelayinMs = 6000 / speedFactor + 300;
        _minExitDelayInMs = (int)Math.Round(minDelayinMs);
        _maxExitDelayInMs = (int)Math.Round(maxDelayinMs);
    }

    private string _validLicenseNumberChars = "DFGHJKLNPRSTXYZ";

    private string GenerateRandomLicenseNumber()
    {
        int type = _rnd.Next(1, 9);
        string kenteken = string.Empty;
        switch (type)
        {
            case 1: // 99-AA-99
                kenteken = string.Format("{0:00}-{1}-{2:00}", _rnd.Next(1, 99), GenerateRandomCharacters(2), _rnd.Next(1, 99));
                break;
            case 2: // AA-99-AA
                kenteken = string.Format("{0}-{1:00}-{2}", GenerateRandomCharacters(2), _rnd.Next(1, 99), GenerateRandomCharacters(2));
                break;
            case 3: // AA-AA-99
                kenteken = string.Format("{0}-{1}-{2:00}", GenerateRandomCharacters(2), GenerateRandomCharacters(2), _rnd.Next(1, 99));
                break;
            case 4: // 99-AA-AA
                kenteken = string.Format("{0:00}-{1}-{2}", _rnd.Next(1, 99), GenerateRandomCharacters(2), GenerateRandomCharacters(2));
                break;
            case 5: // 99-AAA-9
                kenteken = string.Format("{0:00}-{1}-{2}", _rnd.Next(1, 99), GenerateRandomCharacters(3), _rnd.Next(1, 10));
                break;
            case 6: // 9-AAA-99
                kenteken = string.Format("{0}-{1}-{2:00}", _rnd.Next(1, 9), GenerateRandomCharacters(3), _rnd.Next(1, 10));
                break;
            case 7: // AA-999-A
                kenteken = string.Format("{0}-{1:000}-{2}", GenerateRandomCharacters(2), _rnd.Next(1, 999), GenerateRandomCharacters(1));
                break;
            case 8: // A-999-AA
                kenteken = string.Format("{0}-{1:000}-{2}", GenerateRandomCharacters(1), _rnd.Next(1, 999), GenerateRandomCharacters(2));
                break;
        }

        return kenteken;
    }

    private string GenerateRandomCharacters(int aantal)
    {
        char[] chars = new char[aantal];
        for (int i = 0; i < aantal; i++)
        {
            chars[i] = _validLicenseNumberChars[_rnd.Next(_validLicenseNumberChars.Length - 1)];
        }
        return new string(chars);
    }

    #endregion
}
