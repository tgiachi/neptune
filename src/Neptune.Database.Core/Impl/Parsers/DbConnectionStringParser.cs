using System.Text.RegularExpressions;
using Neptune.Database.Core.Data;
using Neptune.Database.Core.Types;

namespace Neptune.Database.Core.Impl.Parsers;

public static class DbConnectionStringParser
{
    public static DataSourceObject Parse(string url)
    {
        var regex = new Regex(@"^(?<scheme>\w+)://(?<user>[^:]+):(?<pass>[^@]+)@(?<host>[^:\/]+):(?<port>\d+)/(?<db>\w+)$");

        var match = regex.Match(url);
        if (!match.Success)
        {
            throw new ArgumentException("Invalid connection string format.");
        }

        var scheme = match.Groups["scheme"].Value;
        var databaseType = scheme.ToLower() switch
        {
            "postgres" or "postgresql" => DatabaseType.PostgreSql,
            "mysql"                    => DatabaseType.MySql,
            "mssql" or "sqlserver"     => DatabaseType.SqlServer,
            "sqlite"                   => DatabaseType.Sqlite,
            _                          => throw new NotSupportedException($"Database type '{scheme}' is not supported.")
        };

        return new DataSourceObject
        {
            ConnectionString = url,
            DatabaseType = databaseType,
            Username = match.Groups["user"].Value,
            Password = match.Groups["pass"].Value,
            Host = match.Groups["host"].Value,
            Port = match.Groups["port"].Value,
            DatabaseName = match.Groups["db"].Value
        };
    }
}
