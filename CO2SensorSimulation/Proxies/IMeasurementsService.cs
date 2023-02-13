namespace CO2SensorSimulation.Proxies;

public interface IMeasurementsService
{
    public Task SendCO2LevelMeasuredAsync(CO2LevelMeasured co2LevelMeasured);
}
