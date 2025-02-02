namespace Finos.CCC.Validator.Models;

internal record Threat
{
    public required string Id { get; set; }

    public required string Title { get; set; }

    public required List<string> Features { get; set; }
}
