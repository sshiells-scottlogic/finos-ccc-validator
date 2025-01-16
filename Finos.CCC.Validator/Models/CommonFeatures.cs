using System.ComponentModel.DataAnnotations;

namespace Finos.CCC.Validator.Models
{
    internal record CommonFeatures
    {
        [Required]
        public required List<Feature> Features { get; set; }
    }
}
