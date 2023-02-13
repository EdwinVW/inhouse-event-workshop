namespace MeasurementsService.Repositories;

public interface IMeasurementsRepository
{
    Task SaveAverageSpeedAsync(AverageSpeed averageSpeed);
    Task<IEnumerable<AverageSpeed>> GetAverageSpeedsAsync(DateTime timestamp);
    Task SaveSamplePointAsync(SamplePoint samplePoint);
    Task<IEnumerable<SamplePoint>> GetSamplePointsAsync();
}
