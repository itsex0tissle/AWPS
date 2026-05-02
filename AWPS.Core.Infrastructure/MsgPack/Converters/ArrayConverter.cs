using ProGaudi.MsgPack.Light;
using System.Diagnostics.CodeAnalysis;

namespace AWPS.Core.Infrastructure.MsgPack.Converters;

public sealed class ArrayConverter<T> : IMsgPackConverter<T[]>
{
    #region Instance
    private IMsgPackConverter<T> ElementConverter { get; set; } = null!; //Init by Initialize(MsgPackContext context)
    #endregion  

    #region IMsgPackConverter<T[]>
    public void Initialize(MsgPackContext context)
    {
        ElementConverter = context.GetConverter<T>();
    }
    public void Write(T[] obj, [NotNull] IMsgPackWriter writer)
    {
        writer.WriteArrayHeader((uint)obj.Length);
        foreach(T item in obj)
        {
            ElementConverter.Write(item, writer);
        }
    }
    public T[] Read([NotNull] IMsgPackReader reader)
    {
        var array = new T[reader.ReadArrayLength() ?? 0];
        for(int index = 0; index < array.Length; index++)
        {
            array[index] = ElementConverter.Read(reader);
        }
        return array;
    }
    #endregion
}