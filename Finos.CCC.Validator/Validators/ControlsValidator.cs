using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal interface IControlsValidator : IValidator;
internal class ControlsValidator : FileParser, IControlsValidator
{
    public async Task<bool> Validate(CommonData commonData)
    {
        var valid = true;
        foreach (var file in commonData.MetaData)
        {
            valid &= await ValidateControl(file.Key, file.Value, commonData);
        }

        return valid;
    }

    private async Task<bool> ValidateControl(string filePath, Metadata metadata, CommonData commonData)
    {
        var valid = true;
        var fullFilePath = Path.Combine(filePath, "controls.yaml");
        if (!File.Exists(fullFilePath))
        {
            Console.WriteLine($"{fullFilePath} not found.");
            return true;
        }

        var controlFile = await ParseYamlFile<ControlsFile>(fullFilePath);

        Console.WriteLine($"Validation of {fullFilePath} Started.");

        valid &= ValidateCommonControls(controlFile, commonData);
        valid &= ValidateControlsId(controlFile, metadata);
        valid &= ValidateTestRequirements(controlFile);

        var threatsFilePath = Path.Combine(filePath, "threats.yaml");

        if (File.Exists(threatsFilePath))
        {
            var threatsFile = await ParseYamlFile<ThreatsFile>(threatsFilePath);
            valid &= ValidateThreats(controlFile, threatsFile);
        }
        else
        {
            Console.WriteLine($"{threatsFilePath} not found - skipping threats validation.");
        }

        Console.WriteLine($"Validation of {fullFilePath} Complete. Status {valid.ToPassOrFail()}.");
        return valid;
    }

    private bool ValidateCommonControls(ControlsFile file, CommonData commonData)
    {
        var valid = true;

        foreach (var control in file.CommonControls)
        {
            if (!commonData.Controls.Contains(control))
            {
                Console.WriteLine($"ERROR: Control {control} is not a valid common control.");
                valid = false;
            }
        }

        return valid;
    }

    private bool ValidateControlsId(ControlsFile file, Metadata metadata)
    {
        var valid = true;

        if (file.Controls == null)
        {
            return valid;
        }

        foreach (var control in file.Controls)
        {
            if (!control.Id.StartsWith(metadata.Id))
            {
                Console.WriteLine($"ERROR: Control {control} does not match Id {metadata.Id} specified in Metadata file.");
                valid = false;
            }
        }

        return valid;
    }

    private bool ValidateThreats(ControlsFile file, ThreatsFile threatsFile)
    {
        var valid = true;

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
                    Console.WriteLine($"ERROR: {control.Id} contains an invalid threat: {threat}.");
                    valid = false;
                }
            }
        }

        return valid;
    }

    private bool ValidateTestRequirements(ControlsFile file)
    {
        var valid = true;

        foreach (var control in file.Controls)
        {
            foreach (var testRequirement in control.TestRequirements)
            {
                if (!testRequirement.Id.StartsWith(control.Id))
                {
                    Console.WriteLine($"ERROR: Test Requirement {testRequirement.Id} doesn't start with control Id: {control.Id}.");
                    valid = false;
                }
            }
        }

        return valid;
    }
}