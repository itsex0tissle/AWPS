using Ardalis.Result;
using System.Text.Json;
using System.Net.Http.Json;
using AWPS.UI.Shared.Helpers;
using AWPS.UI.Shared.Services;
using AWPS.Core.Infrastructure.Data;

namespace AWPS.UI.Web.Client.Services;

public sealed class ApplicationDbInteractor([FromKeyedServices(HttpClientKey.Server)] HttpClient httpClient) : IApplicationDbInteractor
{
    #region Static
    private static Result MapErrorResponse(HttpResponseMessage response)
    {
        return response.StatusCode switch
        {
            System.Net.HttpStatusCode.NotFound => Result.NotFound(),
            System.Net.HttpStatusCode.Unauthorized => Result.Unauthorized(),
            System.Net.HttpStatusCode.BadRequest => Result.Error("Bad request."),
            System.Net.HttpStatusCode.InternalServerError => Result.CriticalError("Server error."),
            _ => Result.CriticalError($"HTTP {(int)response.StatusCode}")
        };
    }
    #endregion

    #region IApplicationDbInteractor
    public async Task<Result<string>> GetCurrentUserEmail()
    {
        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync("/app-db/user-email");
            if (response.IsSuccessStatusCode)
            {
                if (JsonSerializer.Deserialize<Result<string>>(await response.Content.ReadAsStringAsync()) is Result<string> result)
                {
                    return result;
                }
                return Result.CriticalError("Failed to deserialize user response.");
            }

            return MapErrorResponse(response);
        }
        catch (HttpRequestException)
        {
            return Result.Error("Failed to connect to the server.");
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
            using HttpResponseMessage response = await httpClient.DeleteAsync("/app-db/delete-current-user");
            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }

            return MapErrorResponse(response);
        }
        catch (HttpRequestException)
        {
            return Result.Error("Failed to connect to the server.");
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
            using HttpResponseMessage response = await httpClient.GetAsync("/app-db/device-profiles");
            if (response.IsSuccessStatusCode)
            {
                if(JsonSerializer.Deserialize<Result<DeviceProfileEntity[]>>(await response.Content.ReadAsStringAsync()) is Result<DeviceProfileEntity[]> result)
                {
                    return result;
                }
                return Result.CriticalError("Failed to deserialize device profiles response.");
            }

            return MapErrorResponse(response);
        }
        catch (HttpRequestException)
        {
            return Result.Error("Failed to connect to the server.");
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
            using HttpResponseMessage response = await httpClient.GetAsync($"/app-db/device-profile/{device_profile_id}");
            if (response.IsSuccessStatusCode)
            {
                if (JsonSerializer.Deserialize<Result<DeviceProfileEntity>>(await response.Content.ReadAsStringAsync()) is Result<DeviceProfileEntity> result)
                {
                    return result;
                }
                return Result.CriticalError("Failed to deserialize device profile response.");
            }

            return MapErrorResponse(response);
        }
        catch (HttpRequestException)
        {
            return Result.Error("Failed to connect to the server.");
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
            using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/app-db/device-profile", name);
            if (response.IsSuccessStatusCode)
            {
                if (JsonSerializer.Deserialize<Result<DeviceProfileEntity>>(await response.Content.ReadAsStringAsync()) is Result<DeviceProfileEntity> result)
                {
                    return result;
                }
                return Result.CriticalError("Failed to deserialize created device profile response.");
            }

            return MapErrorResponse(response);
        }
        catch (HttpRequestException)
        {
            return Result.Error("Failed to connect to the server.");
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
            using HttpResponseMessage response = await httpClient.PutAsJsonAsync("/app-db/device-profile", profile);
            if (response.IsSuccessStatusCode)
            {
                if (JsonSerializer.Deserialize<Result<DeviceProfileEntity>>(await response.Content.ReadAsStringAsync()) is Result<DeviceProfileEntity> result)
                {
                    return result;
                }
                return Result.CriticalError("Failed to deserialize updated device profile response.");
            }

            return MapErrorResponse(response);
        }
        catch (HttpRequestException)
        {
            return Result.Error("Failed to connect to the server.");
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
            using HttpResponseMessage response = await httpClient.DeleteAsync($"/app-db/device-profile/{device_profile_id}");
            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }

            return MapErrorResponse(response);
        }
        catch (HttpRequestException)
        {
            return Result.Error("Failed to connect to the server.");
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
            using HttpResponseMessage response = await httpClient.GetAsync($"/app-db/telemetry/{device_profile_id}");
            if (response.IsSuccessStatusCode)
            {
                if (JsonSerializer.Deserialize<Result<TelemetryEntity[]>>(await response.Content.ReadAsStringAsync()) is Result<TelemetryEntity[]> result)
                {
                    return result;
                }
                return Result.CriticalError("Failed to deserialize telemetry response.");
            }

            return MapErrorResponse(response);
        }
        catch (HttpRequestException)
        {
            return Result.Error("Failed to connect to the server.");
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
            using HttpResponseMessage response = await httpClient.DeleteAsync($"/app-db/telemetry/{device_profile_id}/clear");
            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }

            return MapErrorResponse(response);
        }
        catch (HttpRequestException)
        {
            return Result.Error("Failed to connect to the server.");
        }
        catch (Exception exc)
        {
            return Result.CriticalError(exc.Message);
        }
    }
    #endregion
}