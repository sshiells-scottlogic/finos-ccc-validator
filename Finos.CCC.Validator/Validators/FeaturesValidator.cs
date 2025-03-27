using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal interface IFeaturesValidator : IValidator;

internal class FeaturesValidator : FileParser, IFeaturesValidator
{
    public async Task<BoolResult> Validate(CommonData commonData)
    {
        var valid = true;
        var errorCount = 0;
        foreach (var file in commonData.MetaData)
        {
            var result = await ValidateFeature(file.Key, file.Value, commonData);
            valid &= result.Valid;
            errorCount += result.ErrorCount;
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    private async Task<BoolResult> ValidateFeature(string filePath, Metadata metadata, CommonData commonData)
    {
        var valid = true;
        var errorCount = 0;
        var fullFilePath = Path.Combine(filePath, "features.yaml");
        if (!File.Exists(fullFilePath))
        {
            Console.WriteLine($"{fullFilePath} not found.");
            return new BoolResult { Valid = valid, ErrorCount = errorCount };
        }
        var featureFile = await ParseYamlFile<FeaturesFile>(fullFilePath);

        Console.WriteLine($"Validation of {fullFilePath} Started.");

        var commonResult = ValidateCommonFeatures(featureFile, commonData);
        var idResult = ValidateFeatureId(featureFile, metadata);

        valid &= commonResult.Valid && idResult.Valid;
        errorCount += commonResult.ErrorCount + idResult.ErrorCount;

        var fileResult = ValidateFile(fullFilePath, commonData);
        valid &= fileResult.Valid;
        errorCount += fileResult.ErrorCount;

        if (valid)
        {
            Console.WriteLine($"Validation of {fullFilePath} Complete. Status {valid.ToPassOrFail()}.");
        }
        else
        {
            ConsoleWriter.WriteError($"Validation of {fullFilePath} Complete. Status {valid.ToPassOrFail()}.");
        }
        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    private BoolResult ValidateCommonFeatures(FeaturesFile file, CommonData commonData)
    {
        var valid = true;
        var errorCount = 0;

        var commonIds = commonData.Features.Select(x => x.Key).ToList();

        foreach (var feature in file.CommonFeatures)
        {
            if (!commonIds.Contains(feature))
            {
                ConsoleWriter.WriteError($"ERROR: Feature {feature} is not a valid common feature.");
                valid = false;
                errorCount++;
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    private BoolResult ValidateFeatureId(FeaturesFile file, Metadata metadata)
    {
        var valid = true;
        var errorCount = 0;

        foreach (var feature in file.Features)
        {
            if (!feature.Id.StartsWith(metadata.Id))
            {
                ConsoleWriter.WriteError($"ERROR: Feature {feature} does not match Id {metadata.Id} specified in Metadata file.");
                valid = false;
                errorCount++;
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    internal BoolResult ValidateFile(string path, CommonData commonData)
    {
        var isValid = true;
        var errorCount = 0;

        var commonDataDict = commonData.ToDictionary();
        var ids = commonDataDict.Keys;

        foreach (var line in File.ReadLines(path))
        {
            foreach (var id in ids)
            {
                if (line.Contains(id))
                {
                    var index = line.IndexOf(id);
                    var rest = line.Substring(index + id.Length).Trim([' ', '#']);
                    if (rest.ToLower() != commonDataDict[id].Title.ToLower())
                    {
                        errorCount++;
                        isValid = false;
                        ConsoleWriter.WriteError($"Invalid comment following Id: {id} has comment '{rest}' but should be '{commonDataDict[id].Title}'");
                    }
                }
            }
        }

        return new BoolResult { Valid = isValid, ErrorCount = errorCount };
    }
}
