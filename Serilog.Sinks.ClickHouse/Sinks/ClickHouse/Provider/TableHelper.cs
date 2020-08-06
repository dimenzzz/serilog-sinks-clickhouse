using System.Collections.Generic;
using System.Linq;

namespace Serilog.Sinks.ClickHouse.Provider
{
    class TableHelper<T>
    {
        public string Create { get; private set; }
        public string Insert { get; private set; }

        public TableHelper(string name, IEnumerable<AdditionalColumn> additionalColumns = null)
        {
            var mapping = ColumnsHelper.Mapping<T>();
            if (additionalColumns != null)
                mapping = mapping.Union(additionalColumns.Select(c => new ColumnAttribute { Name = c.Name, Type = c.Type })).ToList();

            Create = $@"CREATE TABLE IF NOT EXISTS {name} (
                    {string.Join(", ", mapping.Select(m => $"{m.Name} {m.Type}"))}
                )
                ENGINE = MergeTree()
                ORDER BY timestamp";

            Insert = $@"INSERT INTO {name} (
                    {string.Join(", ", mapping.Select(m => $"{m.Name}"))}
                ) VALUES @bulk";
        }
    }
}
