namespace Finos.CCC.Validator.Models;

internal record CommonData
{
    public required Dictionary<string, Metadata> MetaData { get; set; }

    public required List<string> Features { get; set; }
    public required List<string> Threats { get; set; }
    public required List<string> Controls { get; set; }
}
