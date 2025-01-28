namespace Finos.CCC.Validator.Models;

internal record FeaturesFile
{
    public required List<string> CommonFeatures { get; set; }

    public required List<Feature> Features { get; set; }
}
