using System.ComponentModel.DataAnnotations;

namespace Finos.CCC.Validator.Models
{
    internal record Feature
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }
    }
}
