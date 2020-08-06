using System;
using System.Collections.Generic;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.ClickHouse.Provider;

namespace Serilog.Sinks.ClickHouse.Extensions
{
    public static class ClickHouseSinkExtensions
    {
        public static LoggerConfiguration ClickHouse(
            this LoggerSinkConfiguration loggerConfiguration,
            string connectionString,
            string tableName,
            int batchPostingLimit,
            TimeSpan period,
            IEnumerable<AdditionalColumn> additionalColumns = null,
            IFormatProvider formatProvider = null,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose)
        {
            var sink = new ClickHouseSink(
                                connectionString,
                                tableName,
                                batchPostingLimit,
                                period,
                                additionalColumns,
                                formatProvider);
            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}
