using AWPS.IoT.Server.EFCore;
using AWPS.UI.Shared.Services;
using Microsoft.EntityFrameworkCore;
using AWPS.UI.Shared.Services.Models;

namespace AWPS.UI.Web.Services;

public sealed class ApplicationDatabaseInteractor(IDbContextFactory<ApplicationDatabase> DatabaseFactory) : IApplicationDatabaseInteractor
{
    #region IApplicationDatabaseInteractor
    public MeasuringDataModel[] GetMeasuringDataSet()
    {
        using (ApplicationDatabase database = DatabaseFactory.CreateDbContext())
        {
            return database.MeasuringDataSet.AsNoTracking().AsEnumerable().Select(model =>
            {
                return new MeasuringDataModel(DateTime.FromBinary(model.Timestamp).ToLocalTime(), model.Light, model.Moisture, model.Humidity, model.Temperature);
            }).ToArray();
        }
    }
    #endregion
}