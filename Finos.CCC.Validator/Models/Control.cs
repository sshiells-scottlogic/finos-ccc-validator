namespace Finos.CCC.Validator.Models;

internal record Control
{
    public required string Id { get; set; }

    public required string Title { get; set; }

    public required List<string> Threats { get; set; }

    public required List<TestRequirement> TestRequirements { get; set; }
}
