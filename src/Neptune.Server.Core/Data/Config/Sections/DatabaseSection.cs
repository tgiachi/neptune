namespace Neptune.Server.Core.Data.Config.Sections;

public class DatabaseSection
{
    public string ConnectionString { get; set; } = "postgres://postgres:postgres@localhost:5432/neptune_db";

    public bool EnableSqlLogging { get; set; } = false;
}
