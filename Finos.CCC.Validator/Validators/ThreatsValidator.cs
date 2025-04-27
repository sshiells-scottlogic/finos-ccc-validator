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

        var sharedThreatsResult = ValidateSharedThreats(threatFile, commonData);
        var threatIdResult = ValidateThreatId(threatFile, metadata);

        valid &= sharedThreatsResult.Valid && threatIdResult.Valid;
        errorCount += sharedThreatsResult.ErrorCount + threatIdResult.ErrorCount;

        var capabilitiesFilePath = Path.Combine(filePath, "capabilities.yaml");

        CapabilitiesFile? capabilitiesFile = null;

        if (File.Exists(capabilitiesFilePath))
        {
            capabilitiesFile = await ParseYamlFile<CapabilitiesFile>(capabilitiesFilePath);
            var capabilityResult = ValidateCapabilities(threatFile, capabilitiesFile, capabilitiesFilePath);
            valid &= capabilityResult.Valid;
            errorCount += capabilityResult.ErrorCount;
        }
        else
        {
            Console.WriteLine($"{capabilitiesFilePath} not found - skipping capabilities validation.");
        }

        var fileResult = ValidateFile(fullFilePath, commonData, capabilitiesFile);
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

    private BoolResult ValidateSharedThreats(ThreatsFile file, CommonData commonData)
    {
        var valid = true;
        var errorCount = 0;

        var commonIds = commonData.Threats.Select(x => x.Key).ToList();

        var cccSharedThreats = file.SharedThreats.FirstOrDefault(x => x.ReferenceId == "CCC");

        if (cccSharedThreats != null)
        {
            foreach (var threat in cccSharedThreats.Identifiers)
            {
                if (!commonIds.Contains(threat))
                {
                    ConsoleWriter.WriteError($"ERROR: Threat {threat} is not a valid common threat.");
                    valid = false;
                    errorCount++;
                }
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

    private BoolResult ValidateCapabilities(ThreatsFile file, CapabilitiesFile capabilitiesFile, string capabilityFilePath)
    {
        var valid = true;
        var errorCount = 0;

        if (file.Threats == null)
        {
            return new BoolResult { Valid = valid, ErrorCount = errorCount };
        }

        var cccSharedCapabilities = capabilitiesFile.SharedCapabilities.FirstOrDefault(x => x.ReferenceId == "CCC");
        if (cccSharedCapabilities != null)
        {
            var validCapabilities = cccSharedCapabilities.Identifiers.ToList();

            if (capabilitiesFile.Capabilities != null)
            {
                validCapabilities.AddRange(capabilitiesFile.Capabilities.Select(x => x.Id));
            }

            foreach (var threat in file.Threats)
            {
                foreach (var capabilitiy in threat.Capabilities.SelectMany(x => x.Identifiers))
                {
                    if (!validCapabilities.Contains(capabilitiy))
                    {
                        ConsoleWriter.WriteError($"ERROR: {threat.Id} contains an invalid capability: {capabilitiy}. Capability {capabilitiy} is not listed in {capabilityFilePath}.");
                        valid = false;
                        errorCount++;
                    }
                }
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    internal BoolResult ValidateFile(string path, CommonData commonData, CapabilitiesFile? capabilitiesFile)
    {
        var isValid = true;
        var errorCount = 0;

        var commonDataDict = commonData.ToDictionary();
        if (capabilitiesFile != null && capabilitiesFile.Capabilities != null)
        {
            foreach (var capability in capabilitiesFile.Capabilities)
            {
                commonDataDict[capability.Id] = capability;
            }
        }
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
