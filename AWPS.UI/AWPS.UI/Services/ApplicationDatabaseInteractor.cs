using AWPS.UI.Shared.Services;
using AWPS.UI.Shared.Services.Models;

namespace AWPS.UI.Services;

public sealed class ApplicationDatabaseInteractor : IApplicationDatabaseInteractor
{
    #region IApplicationDatabaseInteractor
    public MeasuringDataModel[] GetMeasuringDataSet()
    {
        return [];
    }
    #endregion
}