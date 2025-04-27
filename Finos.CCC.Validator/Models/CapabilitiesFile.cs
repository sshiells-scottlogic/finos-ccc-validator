namespace Finos.CCC.Validator.Models;

internal record CapabilitiesFile
{
    public required List<string> CommonFeatures { get; set; }

    public required List<Capability> Features { get; set; }
}
