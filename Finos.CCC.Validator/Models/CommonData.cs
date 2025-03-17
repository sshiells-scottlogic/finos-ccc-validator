namespace Finos.CCC.Validator.Models;

internal record CommonData
{
    public required Dictionary<string, Metadata> MetaData { get; set; }

    public required List<BaseItem> Features { get; set; }
    public required List<BaseItem> Threats { get; set; }
    public required List<BaseItem> Controls { get; set; }
}
