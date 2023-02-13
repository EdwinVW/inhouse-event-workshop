namespace MeasurementsService.Repositories;

public class InMemoryMeasurementsRepository : IMeasurementsRepository
{
    private readonly List<AverageSpeed> _averageSpeeds;
    private readonly List<SamplePoint> _samplePoints;

    private DateTime _lastSamplePointTimestamp = DateTime.MinValue;

    public InMemoryMeasurementsRepository()
    {
        _averageSpeeds = new List<AverageSpeed>();
        _samplePoints = new List<SamplePoint>();
    }

    public Task<IEnumerable<AverageSpeed>> GetAverageSpeedsAsync(DateTime timestamp)
    {
        // Get average speed readings between the last sample point timestamp
        // and the timestamp of the CO2Level measurement (parameter)
        
        return Task.FromResult(_averageSpeeds.Where(
            s => s.Timestamp >= _lastSamplePointTimestamp && 
            s.Timestamp <= timestamp));
    }

    public Task<IEnumerable<SamplePoint>> GetSamplePointsAsync()
    {
        return Task.FromResult(_samplePoints.Take(50));
    }

    public Task SaveAverageSpeedAsync(AverageSpeed averageSpeed)
    {
        return Task.Run(() => _averageSpeeds.Add(averageSpeed));
    }

    public Task SaveSamplePointAsync(SamplePoint samplePoint)
    {
        _lastSamplePointTimestamp = samplePoint.Timestamp;
        return Task.Run(() => _samplePoints.Add(samplePoint));
    }
}
