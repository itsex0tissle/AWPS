using AWPS.UI.Shared.Services.Models;

namespace AWPS.UI.Shared.Services;

public interface IApplicationDatabaseInteractor
{
    public abstract MeasuringDataModel[] GetMeasuringDataSet();
}