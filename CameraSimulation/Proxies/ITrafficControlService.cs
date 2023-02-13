namespace CameraSimulation.Proxies;

public interface ITrafficControlService
{
    public Task SendVehicleEntryAsync(VehicleRegistered vehicleRegistered);
    public Task SendVehicleExitAsync(VehicleRegistered vehicleRegistered);
}
