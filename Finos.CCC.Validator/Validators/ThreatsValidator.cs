using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal interface IThreatsValidator : IValidator;

internal class ThreatsValidator : FileParser, IThreatsValidator
{
    public async Task<BoolResult> Validate(CommonData commonData)
    {
        var valid = true;
        var errorCount = 0;
        foreach (var file in commonData.MetaData)
        {
            var threatResult = await ValidateThreat(file.Key, file.Value, commonData);
            valid &= threatResult.Valid;
            errorCount += threatResult.ErrorCount;
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    private async Task<BoolResult> ValidateThreat(string filePath, Metadata metadata, CommonData commonData)
    {
        var valid = true;
        var errorCount = 0;

        var fullFilePath = Path.Combine(filePath, "threats.yaml");
        if (!File.Exists(fullFilePath))
        {
            Console.WriteLine($"{fullFilePath} not found.");
            return new BoolResult { Valid = valid, ErrorCount = errorCount };
        }

        var threatFile = await ParseYamlFile<ThreatsFile>(fullFilePath);

        Console.WriteLine($"Validation of {fullFilePath} Started.");

        var commonThreatsResult = ValidateCommonThreats(threatFile, commonData);
        var threatIdResult = ValidateThreatId(threatFile, metadata);

        valid &= commonThreatsResult.Valid && threatIdResult.Valid;
        errorCount += commonThreatsResult.ErrorCount + threatIdResult.ErrorCount;

        var featuresFilePath = Path.Combine(filePath, "features.yaml");

        if (File.Exists(featuresFilePath))
        {
            var featuresFile = await ParseYamlFile<FeaturesFile>(featuresFilePath);
            var featureResult = ValidateFeatures(threatFile, featuresFile, featuresFilePath);
            valid &= featureResult.Valid;
            errorCount += featureResult.ErrorCount;
        }
        else
        {
            Console.WriteLine($"{featuresFilePath} not found - skipping features validation.");
        }

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

    private BoolResult ValidateCommonThreats(ThreatsFile file, CommonData commonData)
    {
        var valid = true;
        var errorCount = 0;

        var commonIds = commonData.Threats.Select(x => x.Id).ToList();

        foreach (var threat in file.CommonThreats)
        {
            if (!commonIds.Contains(threat))
            {
                ConsoleWriter.WriteError($"ERROR: Threat {threat} is not a valid common threat.");
                valid = false;
                errorCount++;
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    private BoolResult ValidateThreatId(ThreatsFile file, Metadata metadata)
    {
        var valid = true;
        var errorCount = 0;

        if (file.Threats == null)
        {
            return new BoolResult { Valid = valid, ErrorCount = errorCount };
        }

        foreach (var threat in file.Threats)
        {
            if (!threat.Id.StartsWith(metadata.Id))
            {
                ConsoleWriter.WriteError($"ERROR: Threat {threat} does not match Id {metadata.Id} specified in Metadata file.");
                valid = false;
                errorCount++;
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    private BoolResult ValidateFeatures(ThreatsFile file, FeaturesFile featuresFile, string featuresFilePath)
    {
        var valid = true;
        var errorCount = 0;

        if (file.Threats == null)
        {
            return new BoolResult { Valid = valid, ErrorCount = errorCount };
        }

        var validFeatures = featuresFile.CommonFeatures.ToList();
        if (featuresFile.Features != null)
        {
            validFeatures.AddRange(featuresFile.Features.Select(x => x.Id));
        }

        foreach (var threat in file.Threats)
        {
            foreach (var feature in threat.Features)
            {
                if (!validFeatures.Contains(feature))
                {
                    ConsoleWriter.WriteError($"ERROR: {threat.Id} contains an invalid feature: {feature}. Feature {feature} is not listed in {featuresFilePath}.");
                    valid = false;
                    errorCount++;
                }
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }
}
