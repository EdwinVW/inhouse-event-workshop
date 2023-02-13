namespace MeasurementsService.Events;

public record struct CO2LevelMeasured(int CO2LevelInPPM, DateTime Timestamp);