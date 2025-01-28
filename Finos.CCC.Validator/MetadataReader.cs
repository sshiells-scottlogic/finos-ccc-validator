using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator;

internal class MetadataReader : FileParser
{
    private const string FileName = "metadata.yaml";

    public async Task<Dictionary<string, Metadata>> LoadMetaData(string targetDir)
    {
        var rawData = await ParseYamlFiles<Metadata>(targetDir, FileName);
        return rawData.ToDictionary(x => Path.GetDirectoryName(x.Key), x => x.Value);
    }
}
