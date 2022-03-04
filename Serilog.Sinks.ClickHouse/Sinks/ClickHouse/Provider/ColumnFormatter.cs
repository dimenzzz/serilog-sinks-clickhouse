using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Serilog.Events;

namespace Serilog.Sinks.ClickHouse.Provider
{
    class ColumnFormatter : IEnumerable
    {
        private static readonly List<PropertyInfo> _props = ColumnsHelper.Props<ColumnFormatter>();

        private readonly LogEvent _message;
        private readonly IFormatProvider _formatProvider;
        private readonly IEnumerable<AdditionalColumn> _additionslColumns;

        [Column(Name = "timestamp", Type = "DateTime")]
        public DateTime Timestamp { get => _message.Timestamp.UtcDateTime; }
        [Column(Name = "level", Type = "Int")]
        public LogEventLevel Level { get => _message.Level; }
        [Column(Name = "message", Type = "String")]
        public string Message { get => _message.RenderMessage(_formatProvider); }
        [Column(Name = "exception", Type = "String")]
        public string Exception { get => _message.Exception?.ToString(); }

        
        public ColumnFormatter(LogEvent message, IFormatProvider formatProvider = null, IEnumerable<AdditionalColumn> additionalColumns = null)
        {
            _message = message;
            _additionslColumns = additionalColumns;
            _formatProvider = formatProvider;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var p in _props)
                yield return p.GetValue(this);

            if (_additionslColumns != null)
            {
                foreach (var col in _additionslColumns)
                {
                    if (!_message.Properties.TryGetValue(col.Name, out var value))
                    {
                        yield return Default(col.Type);
                        continue;
                    }

                    if(!(value is ScalarValue scalarValue))
                    {
                        yield return value.ToString();
                        continue;
                    }

                    yield return scalarValue.Value;
                }
            }
        }

        private object Default(string type)
        {
            return type switch
            {
                "Boolean" => default(bool),
                "UInt" => default(uint),
                "UInt8" => default(byte),
                "UInt16" => default(UInt16),
                "UInt32" => default(UInt32),
                "UInt64" => default(UInt64),
                "Int" => default(int),
                "Int8" => default(sbyte),
                "Int16" => default(Int16),
                "Int32" => default(Int32),
                "Int64" => default(Int64),
                "Float32" => default(float),
                "Float64" => default(double),
                "Single" => default(float),
                "Double" => default(double),
                "DateTime" => default(DateTime),
                "String" => default(String),
                _ => throw new NotSupportedException()
            };
        }
    }
}
