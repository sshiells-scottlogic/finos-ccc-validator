namespace Finos.CCC.Validator.Models;

internal record CommonData
{
    public required Dictionary<string, Metadata> MetaData { get; set; }

    public required Dictionary<string, BaseItem> Features { get; set; }
    public required Dictionary<string, BaseItem> Threats { get; set; }
    public required Dictionary<string, BaseItem> Controls { get; set; }

    public IDictionary<string, BaseItem> ToDictionary()
    {
        var combined = new Dictionary<string, BaseItem>();

        foreach (var item in Features)
        {
            combined[item.Key] = item.Value;
        }

        foreach (var item in Threats)
        {
            combined[item.Key] = item.Value;
        }

        foreach (var item in Controls)
        {
            combined[item.Key] = item.Value;
        }

        return combined;
    }
}
