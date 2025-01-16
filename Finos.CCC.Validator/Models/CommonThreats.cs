using System.ComponentModel.DataAnnotations;

namespace Finos.CCC.Validator.Models;

internal record CommonThreats
{
    [Required]
    public required List<Threat> Threats { get; set; }
}
