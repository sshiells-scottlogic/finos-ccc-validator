using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal interface IControlsValidator : IValidator;
internal class ControlsValidator : FileParser, IControlsValidator
{
    public async Task<BoolResult> Validate(CommonData commonData)
    {
        var valid = true;
        var errorCount = 0;
        foreach (var file in commonData.MetaData)
        {
            var controlResult = await ValidateControl(file.Key, file.Value, commonData);
            valid &= controlResult.Valid;
            errorCount += controlResult.ErrorCount;
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    private async Task<BoolResult> ValidateControl(string filePath, Metadata metadata, CommonData commonData)
    {
        var valid = true;
        var errorCount = 0;
        var fullFilePath = Path.Combine(filePath, "controls.yaml");
        if (!File.Exists(fullFilePath))
        {
            Console.WriteLine($"{fullFilePath} not found.");
            return new BoolResult { Valid = valid, ErrorCount = errorCount };
        }

        var controlFile = await ParseYamlFile<ControlsFile>(fullFilePath);

        Console.WriteLine($"Validation of {fullFilePath} Started.");

        var commonControlsResult = ValidateCommonControls(controlFile, commonData);
        var idsResult = ValidateControlsId(controlFile, metadata);
        var testRequirementsResult = ValidateTestRequirements(controlFile);

        valid &= commonControlsResult.Valid && idsResult.Valid && testRequirementsResult.Valid;
        errorCount += commonControlsResult.ErrorCount + idsResult.ErrorCount + testRequirementsResult.ErrorCount;

        var threatsFilePath = Path.Combine(filePath, "threats.yaml");

        ThreatsFile? threatsFile = null;

        if (File.Exists(threatsFilePath))
        {
            threatsFile = await ParseYamlFile<ThreatsFile>(threatsFilePath);
            var threatsResult = ValidateThreats(controlFile, threatsFile, threatsFilePath);
            valid &= threatsResult.Valid;
            errorCount += threatsResult.ErrorCount;
        }
        else
        {
            Console.WriteLine($"{threatsFilePath} not found - skipping threats validation.");
        }

        var fileResult = ValidateFile(fullFilePath, commonData, threatsFile);
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

    private BoolResult ValidateCommonControls(ControlsFile file, CommonData commonData)
    {
        var valid = true;
        var errorCount = 0;

        var commonIds = commonData.Controls.Select(control => control.Key).ToList();

        foreach (var control in file.CommonControls)
        {
            if (!commonIds.Contains(control))
            {
                ConsoleWriter.WriteError($"ERROR: Control {control} is not a valid common control.");
                valid = false;
                errorCount++;
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    private BoolResult ValidateControlsId(ControlsFile file, Metadata metadata)
    {
        var valid = true;
        var errorCount = 0;

        if (file.Controls == null)
        {
            return new BoolResult { Valid = valid, ErrorCount = errorCount };
        }

        foreach (var control in file.Controls)
        {
            if (!control.Id.StartsWith(metadata.Id))
            {
                ConsoleWriter.WriteError($"ERROR: Control {control} does not match Id {metadata.Id} specified in Metadata file.");
                valid = false;
                errorCount++;
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    private BoolResult ValidateThreats(ControlsFile file, ThreatsFile threatsFile, string threatsFilePath)
    {
        var valid = true;
        var errorCount = 0;

        var validThreats = threatsFile.CommonThreats.ToList();
        if (threatsFile.Threats != null)
        {
            validThreats.AddRange(threatsFile.Threats.Select(x => x.Id));
        }

        foreach (var control in file.Controls)
        {
            foreach (var threat in control.Threats)
            {
                if (!validThreats.Contains(threat))
                {
                    ConsoleWriter.WriteError($"ERROR: {control.Id} contains an invalid threat: {threat}. Threat {threat} was not listed in {threatsFilePath}.");
                    valid = false;
                    errorCount++;
                }
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    private BoolResult ValidateTestRequirements(ControlsFile file)
    {
        var valid = true;
        var errorCount = 0;

        foreach (var control in file.Controls)
        {
            foreach (var testRequirement in control.TestRequirements)
            {
                if (!testRequirement.Id.StartsWith(control.Id))
                {
                    ConsoleWriter.WriteError($"ERROR: Test Requirement {testRequirement.Id} doesn't start with control Id: {control.Id}.");
                    valid = false;
                    errorCount++;
                }
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }

    internal BoolResult ValidateFile(string path, CommonData commonData, ThreatsFile? threatsFile)
    {
        var isValid = true;
        var errorCount = 0;

        var commonDataDict = commonData.ToDictionary();
        var ids = commonDataDict.Keys;

        if (threatsFile != null && threatsFile.Threats != null)
        {
            foreach (var threat in threatsFile.Threats)
            {
                commonDataDict[threat.Id] = threat;
            }
        }

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