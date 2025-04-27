namespace Finos.CCC.Validator.Models;

internal record CapabilitiesFile
{
    public required List<ReferenceList> SharedCapabilities { get; set; }

    public required List<Capability> Capabilities { get; set; }
}
