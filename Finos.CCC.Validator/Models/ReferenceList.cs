namespace Finos.CCC.Validator.Models;

internal record ReferenceList
{
    public required string ReferenceId { get; set; }
    public required List<string> Identifiers { get; set; }
}