using Ardalis.Result;
using AWPS.UI.Shared.Services;
using AWPS.Core.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AWPS.UI.Web.Services;

public sealed class ApplicationDbInteractor(
    ApplicationDbContext dbContext,
    IotServerInteractor iotServerInteractor,
    IHttpContextAccessor httpContextAccessor,
    UserManager<ApplicationUserEntity> userManager
) : IApplicationDbInteractor
{
    #region Instance
    private string? GetCurrentUserId()
    {
        if(httpContextAccessor.HttpContext is not HttpContext httpContext)
        {
            return null;
        }
        return userManager.GetUserId(httpContext.User);
    }
    #endregion

    #region IApplicationDbInteractor
    public async Task<Result<string>> GetCurrentUserEmail()
    {
        if(httpContextAccessor.HttpContext is not HttpContext httpContext)
        {
            return Result.Unauthorized();
        }
        return Result.Success(userManager.GetUserName(httpContext.User) ?? "");
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
            try
            {
                await iotServerInteractor.StartTrackingDevice(profile.Id);
            }
            catch { }

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

            try
            {
                await iotServerInteractor.StopTrackingDevice(device_profile_id);
            }
            catch { }
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