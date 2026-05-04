using Ardalis.Result;
using AWPS.Core.Infrastructure.Data;

namespace AWPS.UI.Shared.Services;

public interface IApplicationDbInteractor
{
    public abstract Task<Result<ApplicationUserEntity>> GetCurrentUser();
    public abstract Task<Result> DeleteCurrentUser();
    public abstract Task<Result<DeviceProfileEntity[]>> GetDeviceProfiles();
    public abstract Task<Result<DeviceProfileEntity>> GetDeviceProfile(string device_profile_id);
    public abstract Task<Result<DeviceProfileEntity>> CreateDeviceProfile(string name);
    public abstract Task<Result<DeviceProfileEntity>> UpdateDeviceProfile(DeviceProfileEntity profile);
    public abstract Task<Result> DeleteDeviceProfile(string device_profile_id);
    public abstract Task<Result<TelemetryEntity[]>> GetTelemetry(string device_profile_id);
    public abstract Task<Result> ClearTelemetry(string device_profile_id);
}