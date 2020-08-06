using System.Collections.Generic;
using System.Reflection;

namespace Serilog.Sinks.ClickHouse.Provider
{
    static class ColumnsHelper
    {
        public static List<ColumnAttribute> Mapping<T>()
        {
            var dict = new List<ColumnAttribute>();
            var props = typeof(T).GetProperties();
            foreach (var p in props)
            {
                var colAttr = p.GetCustomAttribute<ColumnAttribute>();
                if (colAttr != null)
                    dict.Add(colAttr);
            }

            return dict;
        }

        public static List<PropertyInfo> Props<T>()
        {
            var dict = new List<PropertyInfo>();
            var props = typeof(T).GetProperties();
            foreach (var p in props)
            {
                if (p.GetCustomAttribute<ColumnAttribute>() != null)
                    dict.Add(p);
            }

            return dict;
        }
    }
}
