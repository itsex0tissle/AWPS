using Microsoft.AspNetCore.Identity;

namespace AWPS.Core.Infrastructure.Data;

public sealed class ApplicationUserEntity : IdentityUser
{
    //Navigation properties
    public List<DeviceProfileEntity>? DeviceProfiles { get; set; }
}