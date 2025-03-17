namespace Finos.CCC.Validator.Models;

internal record Control : BaseItem
{
    public required List<string> Threats { get; set; }

    public required List<TestRequirement> TestRequirements { get; set; }
}
