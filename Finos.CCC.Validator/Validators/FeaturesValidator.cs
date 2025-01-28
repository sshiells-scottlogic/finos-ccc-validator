using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal interface IFeaturesValidator : IFileParser
{
}

internal class FeaturesValidator : FileParser, IFeaturesValidator
{
    public async Task<bool> Validate(string targetDir)
    {
        //var commonFeatures = await ParseYamlFile<CommonFeatures>(Path.Combine(targetDir, "common-features.yaml"));

        //var commonFeatureMap = commonFeatures.Features.ToDictionary(x => x.Id, x => x.Description);

        //var featureFiles = await ParseYamlFiles<FeaturesFile>(targetDir, "features.yaml");

        var valid = true;

        //foreach (var featureFile in featureFiles)
        //{
        //    valid &= ValidateFeature(featureFile.Key, featureFile.Value, commonFeatureMap);
        //}

        return valid;
    }

    private bool ValidateFeature(string filename, FeaturesFile file, Dictionary<string, string> commonFeatureMap)
    {
        Console.WriteLine($"Validating file : {filename}");

        //

        return true;
    }
}
