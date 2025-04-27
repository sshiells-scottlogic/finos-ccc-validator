namespace Finos.CCC.Validator.Models;

internal record Threat : BaseItem
{
    public required List<ReferenceList> Capabilities { get; set; }
}
