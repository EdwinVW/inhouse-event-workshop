namespace TrafficControlService.Proxies;

public class HttpMeasurementsService : IMeasurementsService
{
    private HttpClient _httpClient;

    public HttpMeasurementsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendAverageSpeedMeasuredAsync(AverageSpeedMeasured averageSpeedMeasured)
    {
        var message = JsonContent.Create<AverageSpeedMeasured>(averageSpeedMeasured);
        await _httpClient.PostAsync("http://localhost:6003/averageSpeed", message);
    }
}
