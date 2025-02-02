namespace Finos.CCC.Validator.Models;

internal record ControlsFile
{
    public required List<string> CommonControls { get; set; }

    public required List<Control> Controls { get; set; }
}
