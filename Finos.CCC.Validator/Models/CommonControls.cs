using System.ComponentModel.DataAnnotations;

namespace Finos.CCC.Validator.Models;

internal record CommonControls
{
    [Required]
    public required List<Control> Controls { get; set; }
}
