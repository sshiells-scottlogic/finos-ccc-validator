namespace Finos.CCC.Validator.Models;

internal record Control : BaseItem
{
    public required List<ReferenceList> ThreatMappings { get; set; }

    public required List<TestRequirement> Requirements { get; set; }
}
