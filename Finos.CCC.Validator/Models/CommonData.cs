namespace Finos.CCC.Validator.Models;

internal record CommonData
{
    public required Dictionary<string, Metadata> MetaData { get; set; }

    public required Dictionary<string, BaseItem> Features { get; set; }
    public required Dictionary<string, BaseItem> Threats { get; set; }
    public required Dictionary<string, BaseItem> Controls { get; set; }
}
