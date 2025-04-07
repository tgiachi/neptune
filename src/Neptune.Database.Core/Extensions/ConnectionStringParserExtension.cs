using Neptune.Database.Core.Data;
using Neptune.Database.Core.Impl.Parsers;

namespace Neptune.Database.Core.Extensions;

public static class ConnectionStringParserExtension
{
    public static DataSourceObject ParseDbConnectionString(this string connectionString)
    {
        return DbConnectionStringParser.Parse(connectionString);
    }
}
