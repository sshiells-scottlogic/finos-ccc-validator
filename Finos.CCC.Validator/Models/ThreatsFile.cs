namespace Finos.CCC.Validator.Models;

internal record ThreatsFile
{
    public required List<string> CommonThreats { get; set; }

    public required List<Threat> Threats { get; set; }
}
