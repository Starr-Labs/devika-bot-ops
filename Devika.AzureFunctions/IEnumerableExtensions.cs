using System.Collections.Generic;
using System.Data;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> Enumerate<T>(this T reader) where T : IDataReader
    {
        using (reader)
            while (reader.Read())
                yield return reader;
    }
}