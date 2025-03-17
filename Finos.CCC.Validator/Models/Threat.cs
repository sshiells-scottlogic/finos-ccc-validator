namespace Finos.CCC.Validator.Models;

internal record Threat : BaseItem
{
    public required List<string> Features { get; set; }
}
