using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal interface IThreatsValidator : IValidator;

internal class ThreatsValidator : FileParser, IThreatsValidator
{
    public async Task<bool> Validate(CommonData commonData)
    {
        var valid = true;
        foreach (var file in commonData.MetaData)
        {
            valid &= await ValidateThreat(file.Key, file.Value, commonData);
        }

        return valid;
    }

    private async Task<bool> ValidateThreat(string filePath, Metadata metadata, CommonData commonData)
    {
        var valid = true;
        var fullFilePath = Path.Combine(filePath, "threats.yaml");
        if (!File.Exists(fullFilePath))
        {
            Console.WriteLine($"{fullFilePath} not found.");
            return true;
        }

        var threatFile = await ParseYamlFile<ThreatsFile>(fullFilePath);

        Console.WriteLine($"Validation of {fullFilePath} Started.");

        valid &= ValidateCommonThreats(threatFile, commonData);
        valid &= ValidateThreatId(threatFile, metadata);

        var featuresFilePath = Path.Combine(filePath, "features.yaml");

        if (File.Exists(featuresFilePath))
        {
            var featuresFile = await ParseYamlFile<FeaturesFile>(featuresFilePath);
            valid &= ValidateFeatures(threatFile, featuresFile);
        }
        else
        {
            Console.WriteLine($"{featuresFilePath} not found - skipping features validation.");
        }

        Console.WriteLine($"Validation of {fullFilePath} Complete. Status {valid.ToPassOrFail()}.");
        return valid;
    }

    private bool ValidateCommonThreats(ThreatsFile file, CommonData commonData)
    {
        var valid = true;

        foreach (var threat in file.CommonThreats)
        {
            if (!commonData.Threats.Contains(threat))
            {
                Console.WriteLine($"ERROR: Threat {threat} is not a valid common threat.");
                valid = false;
            }
        }

        return valid;
    }

    private bool ValidateThreatId(ThreatsFile file, Metadata metadata)
    {
        var valid = true;

        if (file.Threats == null)
        {
            return valid;
        }

        foreach (var threat in file.Threats)
        {
            if (!threat.Id.StartsWith(metadata.Id))
            {
                Console.WriteLine($"ERROR: Threat {threat} does not match Id {metadata.Id} specified in Metadata file.");
                valid = false;
            }
        }

        return valid;
    }

    private bool ValidateFeatures(ThreatsFile file, FeaturesFile featuresFile)
    {
        var valid = true;

        if (file.Threats == null)
        {
            return valid;
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
                    Console.WriteLine($"ERROR: {threat.Id} contains an invalid feature: {feature}.");
                    valid = false;
                }
            }
        }

        return valid;
    }
}
