using Ardalis.Result;
using System.Security.Claims;
using AWPS.UI.Shared.Services;
using AWPS.Core.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AWPS.UI.Web.Services;

public sealed class ApplicationDbInteractor(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IotServerInteractor iotServerInteractor) : IApplicationDbInteractor
{
    #region Instance
    private string? GetCurrentUserId()
    {
        return httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
    #endregion

    #region IApplicationDbInteractor
    public async Task<Result<ApplicationUserEntity>> GetCurrentUser()
    {
        try
        {
            string? userId = GetCurrentUserId();
            if (userId is null)
            {
                return Result.Unauthorized();
            }

            ApplicationUserEntity? user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                return Result.NotFound();
            }

            return Result.Success(user);
        }
        catch (Exception exc)
        {
            return Result.CriticalError(exc.Message);
        }
    }
    public async Task<Result> DeleteCurrentUser()
    {
        try
        {
            string? userId = GetCurrentUserId();
            if (userId is null)
            {
                return Result.Unauthorized();
            }

            ApplicationUserEntity? user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                return Result.NotFound();
            }

            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception exc)
        {
            return Result.CriticalError(exc.Message);
        }
    }
    public async Task<Result<DeviceProfileEntity[]>> GetDeviceProfiles()
    {
        try
        {
            string? userId = GetCurrentUserId();
            if (userId is null)
            {
                return Result.Unauthorized();
            }

            DeviceProfileEntity[] profiles = await dbContext.DeviceProfiles
                .Where(dp => dp.UserId == userId)
                .ToArrayAsync();

            return Result.Success(profiles);
        }
        catch (Exception exc)
        {
            return Result.CriticalError(exc.Message);
        }
    }
    public async Task<Result<DeviceProfileEntity>> GetDeviceProfile(string device_profile_id)
    {
        try
        {
            string? userId = GetCurrentUserId();
            if (userId is null)
            {
                return Result.Unauthorized();
            }

            DeviceProfileEntity? profile = await dbContext.DeviceProfiles
                .FirstOrDefaultAsync(dp => dp.Id == device_profile_id && dp.UserId == userId);

            if (profile is null)
            {
                return Result.NotFound();
            }

            return Result.Success(profile);
        }
        catch (Exception exc)
        {
            return Result.CriticalError(exc.Message);
        }
    }
    public async Task<Result<DeviceProfileEntity>> CreateDeviceProfile(string name)
    {
        try
        {
            string? userId = GetCurrentUserId();
            if (userId is null)
            {
                return Result.Unauthorized();
            }

            DeviceProfileEntity profile = new()
            {
                Name = name,
                UserId = userId,
                LastUpdated = DateTime.UtcNow
            };

            dbContext.DeviceProfiles.Add(profile);
            await dbContext.SaveChangesAsync();
            await iotServerInteractor.StartTrackingDevice(profile.Id);

            return Result.Success(profile);
        }
        catch (Exception exc)
        {
            return Result.CriticalError(exc.Message);
        }
    }
    public async Task<Result<DeviceProfileEntity>> UpdateDeviceProfile(DeviceProfileEntity profile)
    {
        try
        {
            string? userId = GetCurrentUserId();
            if (userId is null)
            {
                return Result.Unauthorized();
            }

            DeviceProfileEntity? existingProfile = await dbContext.DeviceProfiles
                .FirstOrDefaultAsync(dp => dp.Id == profile.Id && dp.UserId == userId);

            if (existingProfile is null)
            {
                return Result.NotFound();
            }

            existingProfile.Name = profile.Name;
            existingProfile.DeviceSettings = profile.DeviceSettings;

            dbContext.DeviceProfiles.Update(existingProfile);
            await dbContext.SaveChangesAsync();

            return Result.Success(existingProfile);
        }
        catch (Exception exc)
        {
            return Result.CriticalError(exc.Message);
        }
    }
    public async Task<Result> DeleteDeviceProfile(string device_profile_id)
    {
        try
        {
            string? userId = GetCurrentUserId();
            if (userId is null)
            {
                return Result.Unauthorized();
            }

            DeviceProfileEntity? profile = await dbContext.DeviceProfiles
                .FirstOrDefaultAsync(dp => dp.Id == device_profile_id && dp.UserId == userId);

            if (profile is null)
            {
                return Result.NotFound();
            }

            await iotServerInteractor.StopTrackingDevice(device_profile_id);
            dbContext.DeviceProfiles.Remove(profile);
            await dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception exc)
        {
            return Result.CriticalError(exc.Message);
        }
    }
    public async Task<Result<TelemetryEntity[]>> GetTelemetry(string device_profile_id)
    {
        try
        {
            string? userId = GetCurrentUserId();
            if (userId is null)
            {
                return Result.Unauthorized();
            }

            DeviceProfileEntity? profile = await dbContext.DeviceProfiles
                .FirstOrDefaultAsync(dp => dp.Id == device_profile_id && dp.UserId == userId);

            if (profile is null)
            {
                return Result.NotFound();
            }

            TelemetryEntity[] telemetry = await dbContext.Telemetry
                .Where(t => t.DeviceProfileId == device_profile_id)
                .ToArrayAsync();

            return Result.Success(telemetry);
        }
        catch (Exception exc)
        {
            return Result.CriticalError(exc.Message);
        }
    }
    public async Task<Result> ClearTelemetry(string device_profile_id)
    {
        try
        {
            string? userId = GetCurrentUserId();
            if (userId is null)
            {
                return Result.Unauthorized();
            }

            DeviceProfileEntity? profile = await dbContext.DeviceProfiles
                .FirstOrDefaultAsync(dp => dp.Id == device_profile_id && dp.UserId == userId);

            if (profile is null)
            {
                return Result.NotFound();
            }

            var telemetry = await dbContext.Telemetry
                .Where(t => t.DeviceProfileId == device_profile_id)
                .ToListAsync();

            dbContext.Telemetry.RemoveRange(telemetry);
            await dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception exc)
        {
            return Result.CriticalError(exc.Message);
        }
    }
    #endregion
}