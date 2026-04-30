using AWPS.UI.Shared.Models;

namespace AWPS.UI.Shared.Services;

public interface IApplicationDatabaseInteractor
{
    public abstract MeasuringDataModel[] GetMeasuringDataSet();
    public abstract void ClearMeasuringData();
}