using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Finos.CCC.Validator.Validators;

public interface IValidator
{
    Task<T> ParseYamlFile<T>(string filename);

    Task<Dictionary<string, T>> ParseYamlFiles<T>(string targetDir, string pattern);
}

public abstract class Validator : IValidator
{
    private IDeserializer _deserializer;

    public Validator()
    {
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    public async Task<T> ParseYamlFile<T>(string filename)
    {
        var contents = await File.ReadAllTextAsync(filename);

        return _deserializer.Deserialize<T>(contents);
    }

    public async Task<Dictionary<string, T>> ParseYamlFiles<T>(string targetDir, string pattern)
    {
        var di = new DirectoryInfo(targetDir);
        var allFiles = di.GetFiles(pattern, SearchOption.AllDirectories);

        Dictionary<string, T> result = [];

        foreach (var file in allFiles)
        {
            result.Add(file.FullName, await ParseYamlFile<T>(file.FullName));
        }

        return result;
    }
}
