using Neptune.Database.Core.Types;

namespace Neptune.Database.Core.Data;

public class DataSourceObject
{
    public string ConnectionString { get; set; }

    public DatabaseType DatabaseType { get; set; }

    public string DatabaseName { get; set; }

    public string Host { get; set; }

    public string Port { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string ToStandardConnectionString()
    {
        return $"Server={Host};Port={Port};Database={DatabaseName};User Id={Username};Password={Password};";
    }
}
