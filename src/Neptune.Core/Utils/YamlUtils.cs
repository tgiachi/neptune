namespace Neptune.Core.Utils;

public static class YamlUtils
{
    public static T? Deserialize<T>(string yaml)
    {
        var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<T>(yaml);
    }

    public static object? Deserialize(string yaml, Type type)
    {
        var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize(yaml, type);
    }

    public static string Serialize<T>(T obj)
    {
        var serializer = new YamlDotNet.Serialization.SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention.Instance)
            .Build();

        return serializer.Serialize(obj);
    }
}
