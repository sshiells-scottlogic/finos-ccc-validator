using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal interface IFeaturesValidator : IValidator;

internal class FeaturesValidator : FileParser, IFeaturesValidator
{
    public async Task<bool> Validate(CommonData commonData)
    {
        var valid = true;
        foreach (var file in commonData.MetaData)
        {
            valid &= await ValidateFeature(file.Key, file.Value, commonData);
        }

        return valid;
    }

    private async Task<bool> ValidateFeature(string filePath, Metadata metadata, CommonData commonData)
    {
        var valid = true;
        var fullFilePath = Path.Combine(filePath, "features.yaml");
        var featureFile = await ParseYamlFile<FeaturesFile>(fullFilePath);

        Console.WriteLine($"Validation of {fullFilePath} Started.");

        valid &= ValidateCommonFeatures(featureFile, commonData);
        valid &= ValidateFeatureId(featureFile, metadata);

        Console.WriteLine($"Validation of {fullFilePath} Complete. Status {valid.ToPassOrFail()}.");
        return valid;
    }

    private bool ValidateCommonFeatures(FeaturesFile file, CommonData commonData)
    {
        var valid = true;

        foreach (var feature in file.CommonFeatures)
        {
            if (!commonData.Features.Contains(feature))
            {
                Console.WriteLine($"ERROR: Feature {feature} is not a valid common feature.");
                valid = false;
            }
        }

        return valid;
    }

    private bool ValidateFeatureId(FeaturesFile file, Metadata metadata)
    {
        var valid = true;

        foreach (var feature in file.Features)
        {
            if (!feature.Id.StartsWith(metadata.Id))
            {
                Console.WriteLine($"ERROR: Feature {feature} does not match Id {metadata.Id} specified in Metadata file.");
                valid = false;
            }
        }

        return valid;
    }
}
