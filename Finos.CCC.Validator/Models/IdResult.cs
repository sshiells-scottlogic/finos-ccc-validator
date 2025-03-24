namespace Finos.CCC.Validator.Models;

public record IdResult
{
    public bool Valid { get; set; }
    public Dictionary<string, BaseItem> Ids { get; set; }
    public int ErrorCount { get; set; }
}