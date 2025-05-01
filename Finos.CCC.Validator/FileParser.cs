using Finos.CCC.Validator.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Finos.CCC.Validator;
public interface IFileParser
{
    Task<T> ParseYamlFile<T>(string filename);

    Task<Dictionary<string, T>> ParseYamlFiles<T>(string targetDir, string pattern);
}

public abstract class FileParser : IFileParser
{
    private IDeserializer _deserializer;

    public FileParser()
    {
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
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

    public BoolResult ValidateComments(string path, IDictionary<string, BaseItem> commonDataDict)
    {
        var errorCount = 0;
        var isValid = true;
        var allLines = File.ReadAllLines(path);
        for (var i = 0; i < allLines.Length; i++)
        {
            var line = allLines[i];
            foreach (var id in commonDataDict.Keys)
            {
                if (line.Contains(id))
                {
                    var index = line.IndexOf(id);
                    var rest = line.Substring(index + id.Length).Trim([' ', '#']);
                    if (rest.EndsWith("|"))
                    {
                        rest = rest.Trim(['|', ' ']);
                        if (i < allLines.Length - 1)
                        {
                            var nextLine = allLines[i + 1].Trim([' ']);
                            if (nextLine.StartsWith("#"))
                            {
                                rest = $"{rest} {nextLine.Trim([' ', '#'])}";
                            }
                        }
                    }
                    if (rest.ToLower() != commonDataDict[id].Title.ToLower())
                    {
                        errorCount++;
                        isValid = false;
                        ConsoleWriter.WriteError($"Invalid comment following Id: {id} has comment '{rest}' but should be '{commonDataDict[id].Title}'");
                    }
                }
            }
        }
        return new BoolResult { ErrorCount = errorCount, Valid = isValid };
    }
}
