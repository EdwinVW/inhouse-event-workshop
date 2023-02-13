namespace MeasurementsService.Models;

public record struct SamplePoint(int OverallAverageSpeedInKmh, int CO2LevelInPPM, DateTime Timestamp);