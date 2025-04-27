namespace Finos.CCC.Validator.Models;

internal record ControlFamily
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required List<Control> Controls { get; set; }
}
