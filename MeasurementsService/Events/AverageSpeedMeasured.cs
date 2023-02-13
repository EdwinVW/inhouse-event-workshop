namespace MeasurementsService.Events;

public record struct AverageSpeedMeasured(int AverageSpeedInInKmh, DateTime Timestamp);