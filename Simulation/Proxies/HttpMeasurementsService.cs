namespace Simulation.Proxies;

public class HttpMeasurementsService : IMeasurementsService
{
    private HttpClient _httpClient;

    public HttpMeasurementsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendCO2LevelMeasuredAsync(CO2LevelMeasured co2LevelMeasured)
    {
        var message = JsonContent.Create<CO2LevelMeasured>(co2LevelMeasured);
        await _httpClient.PostAsync("http://localhost:6003/co2Level", message);
    }
}
