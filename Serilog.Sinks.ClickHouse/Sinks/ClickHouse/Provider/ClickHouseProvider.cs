using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClickHouse.Ado;
using Serilog.Debugging;

namespace Serilog.Sinks.ClickHouse.Provider
{
    class ClickHouseProvider<TColumnFormatter>
    {
        private readonly string _connectionString;
        private readonly TableHelper<TColumnFormatter> _table;

        public ClickHouseProvider(
            string tableName, 
            string connectionString, 
            IEnumerable<AdditionalColumn> additionalColumns = null, 
            bool autoCreateSqlTable = true)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            _table = new TableHelper<TColumnFormatter>(tableName, additionalColumns);
            _connectionString = connectionString;

            if (autoCreateSqlTable)
            {
                try
                {
                    Execute(_table.Create);
                }
                catch (Exception ex)
                {
                    SelfLog.WriteLine($"Exception creating table {tableName}:\n{ex}");
                }
            }
            
        }

        public async Task FlushAsync(IEnumerable<TColumnFormatter> buff)
        {
            if (buff.Any())
            {
                using var connection = new ClickHouseConnection(new ClickHouseConnectionSettings(_connectionString));
                await connection.OpenAsync();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = _table.Insert;
                cmd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "bulk",
                    Value = buff
                });
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public void Execute(string command)
        {
            using var connection = new ClickHouseConnection(new ClickHouseConnectionSettings(_connectionString));
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = command;
            cmd.ExecuteNonQuery();
        }
    }
}
