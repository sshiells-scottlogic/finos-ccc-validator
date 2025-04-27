namespace Finos.CCC.Validator.Models;

internal record CommonCapabilities
{
    public required List<Capability> Capabilities { get; set; }
}
