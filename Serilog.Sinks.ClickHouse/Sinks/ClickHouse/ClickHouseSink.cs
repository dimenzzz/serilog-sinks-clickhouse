using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;
using Serilog.Debugging;
using Serilog.Sinks.ClickHouse.Provider;

namespace Serilog.Sinks.ClickHouse
{
    public class ClickHouseSink : IBatchedLogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly ClickHouseProvider<ColumnFormatter> _provider;
        private readonly IEnumerable<AdditionalColumn> _additionalColumns;

        public ClickHouseSink(
            string connectionString,
            string tableName,
            IEnumerable<AdditionalColumn> additionalColumns = null,
            IFormatProvider formatProvider = null,
            bool autoCreateSqlTable = true)
        {
            _additionalColumns = additionalColumns;
            _formatProvider = formatProvider;
            _provider = new ClickHouseProvider<ColumnFormatter>(tableName, connectionString, additionalColumns, autoCreateSqlTable);
        }

        public async Task EmitBatchAsync(IReadOnlyCollection<LogEvent> events)
        {
            try
            {
                await _provider.FlushAsync(events.Select(e => new ColumnFormatter(e, _formatProvider, _additionalColumns)));
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine("Unable to write {0} log events to the database due to following error: {1}", events.Count(), ex.Message);
            }
        }

        public Task OnEmptyBatchAsync() => Task.CompletedTask;
    }
}
