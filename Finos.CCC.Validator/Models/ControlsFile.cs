namespace Finos.CCC.Validator.Models;

internal record ControlsFile
{
    public required List<ReferenceList> SharedControls { get; set; }

    public required List<ControlFamily> ControlFamilies { get; set; }
}
