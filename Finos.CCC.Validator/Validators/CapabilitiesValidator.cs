using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal interface ICapabilitiesValidator : IValidator;

internal class CapabilitiesValidator : FileParser, ICapabilitiesValidator
{
    public async Task<BoolResult> Validate(CommonData commonData)
    {
        var valid = true;
        var errorCount = 0;
        foreach (var file in commonData.MetaData)
        {
            var result = await ValidateCapability(file.Key, file.Value, commonData);
            valid &= result.Valid;
            errorCount += result.ErrorCount;
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    private async Task<BoolResult> ValidateCapability(string filePath, Metadata metadata, CommonData commonData)
    {
        var valid = true;
        var errorCount = 0;
        var fullFilePath = Path.Combine(filePath, "capabilities.yaml");
        if (!File.Exists(fullFilePath))
        {
            Console.WriteLine($"{fullFilePath} not found.");
            return new BoolResult { Valid = valid, ErrorCount = errorCount };
        }
        var capabilityFile = await ParseYamlFile<CapabilitiesFile>(fullFilePath);

        Console.WriteLine($"Validation of {fullFilePath} Started.");

        var commonResult = ValidateCommonCapabilities(capabilityFile, commonData);
        var idResult = ValidateCapabilitiyId(capabilityFile, metadata);

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

    private BoolResult ValidateCommonCapabilities(CapabilitiesFile file, CommonData commonData)
    {
        var valid = true;
        var errorCount = 0;

        var commonIds = commonData.Capabilities.Select(x => x.Key).ToList();

        var cccSharedCapabilities = file.SharedCapabilities.FirstOrDefault(x => x.ReferenceId == "CCC");

        if (cccSharedCapabilities != null)
        {
            foreach (var capability in cccSharedCapabilities.Identifiers)
            {
                if (!commonIds.Contains(capability))
                {
                    ConsoleWriter.WriteError($"ERROR: Capability {capability} is not a valid common capability.");
                    valid = false;
                    errorCount++;
                }
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    private BoolResult ValidateCapabilitiyId(CapabilitiesFile file, Metadata metadata)
    {
        var valid = true;
        var errorCount = 0;

        foreach (var capability in file.Capabilities)
        {
            if (!capability.Id.StartsWith(metadata.Id))
            {
                ConsoleWriter.WriteError($"ERROR: Capability {capability} does not match Id {metadata.Id} specified in Metadata file.");
                valid = false;
                errorCount++;
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    internal BoolResult ValidateFile(string path, CommonData commonData)
    {
        var commonDataDict = commonData.ToDictionary();

        return ValidateComments(path, commonDataDict);
    }
}
