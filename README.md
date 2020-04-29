# serilog-sinks-clickhouse

Usage:
```C#
var log = new LoggerConfiguration()
  .WriteTo.ClickHouse(
      "Compress=False;CheckCompressedHash=False;Compressor=lz4;Host=localhost;Port=9000;Database=test_db;User=default;Password=",
      "test_db.logs",
      50,
      TimeSpan.FromSeconds(30),
      new List<AdditionalColumn>
      {
          new AdditionalColumn { Name = "source", Type = "String" },
          new AdditionalColumn { Name = "user", Type = "String" }
      },
      restrictedToMinimumLevel: LogEventLevel.Information
  ).CreateLogger();
 ``` 
