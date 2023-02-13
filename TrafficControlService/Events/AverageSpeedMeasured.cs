namespace TrafficControlService.Events;

public record struct AverageSpeedMeasured(int AverageSpeedInInKmh, DateTime Timestamp);