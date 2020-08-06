using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using Serilog.Sinks.ClickHouse.Provider;
using Serilog.Debugging;

namespace Serilog.Sinks.ClickHouse
{
    public class ClickHouseSink : PeriodicBatchingSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly ClickHouseProvider<ColumnFormatter> _provider;
        private readonly IEnumerable<AdditionalColumn> _additionalColumns;

        public ClickHouseSink(
            string connectionString,
            string tableName,
            int batchPostingLimit,
            TimeSpan period,
            IEnumerable<AdditionalColumn> additionalColumns = null,
            IFormatProvider formatProvider = null,
            bool autoCreateSqlTable = true) : 
            base(batchPostingLimit, period)
        {
            _additionalColumns = additionalColumns;
            _formatProvider = formatProvider;
            _provider = new ClickHouseProvider<ColumnFormatter>(tableName, connectionString, additionalColumns, autoCreateSqlTable);
        }

        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            try
            {
                _provider.Flush(events.Select(e => new ColumnFormatter(e, _formatProvider, _additionalColumns)));
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine("Unable to write {0} log events to the database due to following error: {1}", events.Count(), ex.Message);
            }
        }
    }
}
