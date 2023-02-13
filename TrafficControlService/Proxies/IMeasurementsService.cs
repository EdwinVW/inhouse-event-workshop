namespace TrafficControlService.Proxies;

public interface IMeasurementsService
{
    public Task SendAverageSpeedMeasuredAsync(AverageSpeedMeasured averageSpeedMeasured);
}
