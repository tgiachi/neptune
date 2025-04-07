using Neptune.Database.Core.Types;
using DataType = FreeSql.DataType;

namespace Neptune.Database.Core.Extensions;

public static class DbTypeConverterExtension
{
    public static DataType ToDataType(this DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.Sqlite     => DataType.Sqlite,
            DatabaseType.PostgreSql => DataType.PostgreSQL,
            _                       => throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null)
        };
    }
}
