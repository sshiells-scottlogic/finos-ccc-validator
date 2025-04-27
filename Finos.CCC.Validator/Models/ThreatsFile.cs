namespace Finos.CCC.Validator.Models;

internal record ThreatsFile
{
    public required List<ReferenceList> SharedThreats { get; set; }

    public List<Threat> Threats { get; set; }
}
