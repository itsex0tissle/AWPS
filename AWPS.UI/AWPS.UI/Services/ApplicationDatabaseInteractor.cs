using AWPS.UI.Shared.Models;
using AWPS.UI.Shared.Services;

namespace AWPS.UI.Services;

public sealed class ApplicationDatabaseInteractor : IApplicationDatabaseInteractor
{
    #region IApplicationDatabaseInteractor
    public MeasuringDataModel[] GetMeasuringDataSet()
    {
        return [];
    }
    public void ClearMeasuringData()
    {

    }
    #endregion
}