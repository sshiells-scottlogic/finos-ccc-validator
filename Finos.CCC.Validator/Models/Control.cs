using System.ComponentModel.DataAnnotations;

namespace Finos.CCC.Validator.Models;

internal record Control
{
    [Required]
    public required string Id { get; set; }

    [Required]
    public required string Title { get; set; }

    [Required]
    public required List<string> Threats { get; set; }
}
