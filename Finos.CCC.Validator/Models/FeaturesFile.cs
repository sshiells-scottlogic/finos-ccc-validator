using System.ComponentModel.DataAnnotations;

namespace Finos.CCC.Validator.Models;

internal record FeaturesFile
{
    [Required]
    public required List<string> CommonFeatures { get; set; }

    [Required]
    public required List<Feature> Features { get; set; }
}
