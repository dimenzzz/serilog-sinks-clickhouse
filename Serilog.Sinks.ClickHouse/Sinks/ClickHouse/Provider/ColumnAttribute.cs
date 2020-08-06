using System;

namespace Serilog.Sinks.ClickHouse.Provider
{
    [AttributeUsage(AttributeTargets.Property)]
    class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
