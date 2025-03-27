namespace Finos.CCC.Validator.Models;

public record BaseItem
{
    public required string Id { get; set; }

    public required string Title { get; set; }
}